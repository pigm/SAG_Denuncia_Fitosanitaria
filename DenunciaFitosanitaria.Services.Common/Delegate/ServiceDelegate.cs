using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DenunciaFitosanicaria.Services.Common.Result;
using DenunciaFitosanitaria.Data.Common.Models;
using DenunciaFitosanitaria.Data.Common.Models.Maps;
using DenunciaFitosanitaria.Services.Common.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DenunciaFitosanitaria.Services.Common.Delegate
{
    public class ServiceDelegate
    {
        
        static Dictionary<String, HttpWebRequest> concurrentRequests = new Dictionary<string, HttpWebRequest>();
        static readonly HttpClient client = new HttpClient();
        static ServiceDelegate instance = null;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static ServiceDelegate Instance
        {
            get
            {
                if (instance == null)
                    instance = new ServiceDelegate();

                return instance;
            }
        }


        /// <summary>
        /// Guardars the convenio inbox.
        /// </summary>
        /// <returns>The convenio inbox.</returns>
        /// <param name="parameters">Parameters.</param>
        public async Task<ServiceResult> GetToken()
        {
            string baseUrl = ConfigProperties.API_ENDPOINT+ConfigProperties.PATH_TOKEN;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var values = new Dictionary<string, string>
            {
                { "user", ConfigProperties.USER_TOKEN },
                { "password", ConfigProperties.PASS_TOKEN }
            };
            var content = new FormUrlEncodedContent(values);
            client.Timeout = TimeSpan.FromSeconds(10);
            ServiceResult result = new ServiceResult();
            try{
                var response = await client.PostAsync(baseUrl, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    JObject responseObj = JObject.Parse(responseString);

                    var status = responseObj["header"]["description"].ToString();
                    if (status == "Success")
                    {
                        var token = responseObj["header"]["token"];
                        result.Success = true;
                        result.TokenResponse = token.ToString();
                    }else{
                        result.Success = false;
                        result.Response = status;
                    }
                }
                else
                {
                    result.Success = false;
                    result.Message = "";
                }
            }catch(Exception e){
                result.Success = false;
                result.Message = e.Message;
            }
            return result;
        }

        /// <summary>
        /// Gets the tipos denuncia.
        /// </summary>
        /// <returns>The tipos denuncia.</returns>
        public async Task<ServiceResult> GetTiposDenuncia(string token){
            string baseUrl = ConfigProperties.API_ENDPOINT + ConfigProperties.PATH_TIPO_DENUNCIA;
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", token);
            var values = new Dictionary<string, string>
            {
                { "estado", "true" }
            };
            var content = new FormUrlEncodedContent(values);
            ServiceResult result = new ServiceResult();
            try
            {
                var response = await client.PostAsync(baseUrl, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    JObject responseObj = JObject.Parse(responseString);

                    var status = responseObj["header"]["description"].ToString();
                    if (status == "Success")
                    {
                        var tokenNew = responseObj["header"]["token"];
                        result.Success = true;
                        result.TokenResponse = tokenNew.ToString();

                        JEnumerable<JToken> tokens = responseObj["response"]["body"].Children();
                        List<TipoDenuncia> listTipos = new List<TipoDenuncia>();
                        foreach (JToken tk in tokens)
                        {
                            TipoDenuncia tipoDenuncia = JsonConvert.DeserializeObject<TipoDenuncia>(tk.ToString());
                            listTipos.Add(tipoDenuncia);
                        }

                        result.Response = listTipos;
                    }
                    else
                    {
                        result.Success = false;
                        result.Response = status;
                    }
                }
                else
                {
                    result.Success = false;
                    result.Message = "";
                }
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Message = e.Message;
            }
            return result;
        }


        /// <summary>
        /// Gets the categorias.
        /// </summary>
        /// <returns>The categorias.</returns>
        /// <param name="token">Token.</param>
        public async Task<ServiceResult> GetCategorias(string token)
        {
            string baseUrl = ConfigProperties.API_ENDPOINT + ConfigProperties.PATH_CATEGORIA;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Add("Authorization", token);
            var values = new Dictionary<string, string>
            {
                { "estado", "true" }
            };
            var content = new FormUrlEncodedContent(values);
            ServiceResult result = new ServiceResult();
            try
            {
                var response = await client.PostAsync(baseUrl, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    JObject responseObj = JObject.Parse(responseString);

                    var status = responseObj["header"]["description"].ToString();
                    if (status == "Success")
                    {
                        var tokenNew = responseObj["header"]["token"];
                        result.Success = true;
                        result.TokenResponse = tokenNew.ToString();

                        JEnumerable<JToken> tokens = responseObj["response"]["body"].Children();
                        List<Categoria> listCat = new List<Categoria>();
                        foreach (JToken tk in tokens)
                        {
                            Categoria categoria = JsonConvert.DeserializeObject<Categoria>(tk.ToString());
                            var imageReq = await GetImage(categoria.ImagenUrl, token);
                            if(imageReq.Success){
                                categoria.ImagenEncrypt = (String)imageReq.Response;
                                listCat.Add(categoria);
                            }
                        }

                        result.Response = listCat;
                    }
                    else
                    {
                        result.Success = false;
                        result.Response = status;
                    }
                }
                else
                {
                    result.Success = false;
                    result.Message = "";
                }
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Message = e.Message;
            }
            return result;
        }


        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <returns>The image.</returns>
        /// <param name="token">Token.</param>
        public async Task<ServiceResult> GetImage(string urlImage,string token)
        {
            string baseUrl = ConfigProperties.API_ENDPOINT + ConfigProperties.PATH_IMAGE;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Add("Authorization", token);
            var values = new Dictionary<string, string>
            {
                { "formato", "1" },
                { "url", urlImage }
            };
            var content = new FormUrlEncodedContent(values);
            ServiceResult result = new ServiceResult();
            try
            {
                var response = await client.PostAsync(baseUrl, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    JObject responseObj = JObject.Parse(responseString);

                    var status = responseObj["header"]["description"].ToString();
                    if (status == "Success")
                    {
                        var tokenNew = responseObj["header"]["token"];
                        result.Success = true;
                        result.TokenResponse = tokenNew.ToString();
                        result.Response = (String)responseObj["response"]["body"];;
                    }
                    else
                    {
                        result.Success = false;
                        result.Response = status;
                    }
                }
                else
                {
                    result.Success = false;
                    result.Message = "";
                }
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Message = e.Message;
            }
            return result;
        }


        /// <summary>
        /// Gets the subcategorias.
        /// </summary>
        /// <returns>The subcategorias.</returns>
        /// <param name="token">Token.</param>
        /// <param name="codCat">Cod cat.</param>
        public async Task<ServiceResult> GetSubcategorias(string token,int codCat)
        {
            string baseUrl = ConfigProperties.API_ENDPOINT + ConfigProperties.PATH_SUBCATEGORIA;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Add("Authorization", token);
            var values = new Dictionary<string, string>
            {
                { "estado", "true" },
                { "IdCategoria", codCat.ToString() }
            };
            var content = new FormUrlEncodedContent(values);
            ServiceResult result = new ServiceResult();
            try
            {
                var response = await client.PostAsync(baseUrl, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    JObject responseObj = JObject.Parse(responseString);

                    var status = responseObj["header"]["description"].ToString();
                    if (status == "Success")
                    {
                        var tokenNew = responseObj["header"]["token"];
                        result.Success = true;
                        result.TokenResponse = tokenNew.ToString();

                        JEnumerable<JToken> tokens = responseObj["response"]["body"].Children();
                        List<SubCategoria> listSubCat = new List<SubCategoria>();
                        foreach (JToken tk in tokens)
                        {
                            SubCategoria subCategoria = JsonConvert.DeserializeObject<SubCategoria>(tk.ToString());
                            var imageReq = await GetImage(subCategoria.ImagenUrl, token);
                            if (imageReq.Success)
                            {
                                subCategoria.ImagenEncrypt = (String)imageReq.Response;
                                listSubCat.Add(subCategoria);
                            }
                        }

                        result.Response = listSubCat;
                    }
                    else
                    {
                        result.Success = false;
                        result.Response = status;
                    }
                }
                else
                {
                    result.Success = false;
                    result.Message = "";
                }
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Message = e.Message;
            }
            return result;
        }


      /// <summary>
      /// Gets the subcategoria detalle imagen.
      /// </summary>
      /// <returns>The subcategoria detalle.</returns>
      /// <param name="token">Token.</param>
      /// <param name="codSub">Cod sub.</param>
        public async Task<ServiceResult> GetSubcategoriaDetalle(string token, int codSub)
        {
            string baseUrl = ConfigProperties.API_ENDPOINT + ConfigProperties.PATH_SUBCATEGORIA_DETALLE;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Add("Authorization", token);
            var values = new Dictionary<string, string>
            {
                { "idsubcategoria", codSub.ToString() }
            };
            var content = new FormUrlEncodedContent(values);
            ServiceResult result = new ServiceResult();
            try
            {
                var response = await client.PostAsync(baseUrl, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    JObject responseObj = JObject.Parse(responseString);

                    var status = responseObj["header"]["description"].ToString();
                    if (status == "Success")
                    {
                        var tokenNew = responseObj["header"]["token"];
                        result.Success = true;
                        result.TokenResponse = tokenNew.ToString();

                        var detalle = responseObj["response"]["body"].ToString();
                        SubCategoriaDetalle subcategoria = new SubCategoriaDetalle();
                        JEnumerable<JToken> tokens = responseObj["response"]["body"].Children();
                        foreach (JToken tk in tokens)
                        {
                            subcategoria = JsonConvert.DeserializeObject<SubCategoriaDetalle>(tk.ToString());
                            var imageReq1 = await GetImage(subcategoria.Imagen1, token);
                            if (imageReq1.Success)
                            {
                                subcategoria.ImagenEncrypt1 = (String)imageReq1.Response;
                            }

                            var imageReq2 = await GetImage(subcategoria.Imagen2, token);
                            if (imageReq2.Success)
                            {
                                subcategoria.ImagenEncrypt2 = (String)imageReq2.Response;
                            }

                            var imageReq3 = await GetImage(subcategoria.Imagen3, token);
                            if (imageReq3.Success)
                            {
                                subcategoria.ImagenEncrypt3 = (String)imageReq3.Response;
                            }

                            var imageReq4 = await GetImage(subcategoria.Imagen4, token);
                            if (imageReq4.Success)
                            {
                                subcategoria.ImagenEncrypt4 = (String)imageReq4.Response;
                            }
                        }

                        result.Response = subcategoria;
                    }
                    else
                    {
                        result.Success = false;
                        result.Response = status;
                    }
                }
                else
                {
                    result.Success = false;
                    result.Message = "";
                }
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Message = e.Message;
            }
            return result;
        }



        /// <summary>
        /// Inserts the denuncia.
        /// </summary>
        /// <returns>The denuncia.</returns>
        /// <param name="denuncia">Denuncia.</param>
        /// <param name="token">Token.</param>
        public async Task<ServiceResult> InsertDenuncia(Denuncia denuncia,string token)
        {
            string baseUrl = ConfigProperties.API_ENDPOINT + ConfigProperties.PATH_INSERT_DENUNCIA;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Add("Authorization", token);
            var jsonObj = JsonConvert.SerializeObject(denuncia);
            var dictionaryObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonObj);
            var content = new FormUrlEncodedContent(dictionaryObj);
            ServiceResult result = new ServiceResult();
            try
            {
                var response = await client.PostAsync(baseUrl, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    JObject responseObj = JObject.Parse(responseString);

                    var status = responseObj["header"]["description"].ToString();
                    if (status == "Success")
                    {
                        var tokenNewInst = responseObj["header"]["token"];
                        result.Success = true;
                        result.TokenResponse = tokenNewInst.ToString();
                        result.Response = (String)responseObj["response"]["body"]; ;
                    }
                    else
                    {
                        result.Success = false;
                        result.Response = status;
                    }
                }
                else
                {
                    result.Success = false;
                    result.Message = "";
                }
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Message = e.Message;
            }
            return result;
        }


        /// <summary>
        /// Updates the denuncia.
        /// </summary>
        /// <returns>The denuncia.</returns>
        /// <param name="denuncia">Denuncia.</param>
        /// <param name="correo">Correo.</param>
        /// <param name="telefono">Telefono.</param>
        /// <param name="token">Token.</param>
        public async Task<ServiceResult> UpdateDenuncia(string denuncia,string correo,string telefono, string token)
        {
            string baseUrl = ConfigProperties.API_ENDPOINT + ConfigProperties.PATH_UPDATE_DENUNCIA;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Add("Authorization", token);
            var values = new Dictionary<string, string>
            {
                { "iddenuncia", denuncia },
                { "CorreoContacto", correo },
                { "TelefonoContacto", telefono }
            };

            var content = new FormUrlEncodedContent(values);
            ServiceResult result = new ServiceResult();
            try
            {
                var response = await client.PostAsync(baseUrl, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    JObject responseObj = JObject.Parse(responseString);

                    var status = responseObj["header"]["description"].ToString();
                    if (status == "Success")
                    {
                        var tokenNewInst = responseObj["header"]["token"];
                        result.Success = true;
                        result.TokenResponse = tokenNewInst.ToString();
                        result.Response = (bool)responseObj["response"]["body"]; ;
                    }
                    else
                    {
                        result.Success = false;
                        result.Response = status;
                    }
                }
                else
                {
                    result.Success = false;
                    result.Message = "";
                }
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Message = e.Message;
            }
            return result;
        }

        /// <summary>
        /// Inserts the audio.
        /// </summary>
        /// <returns>The audio.</returns>
        /// <param name="audio">Audio.</param>
        /// <param name="path">Path.</param>
        /// <param name="audioName">Audio name.</param>
        /// <param name="token">Token.</param>
        public async Task<ServiceResult> InsertAudio(Stream audio, string path,string audioName,string token)
        {
            string baseUrl = ConfigProperties.API_ENDPOINT + ConfigProperties.PATH_INSERT_AUDIO +"?filename="+ audioName;
            ServiceResult result = new ServiceResult();
            try
            {
                var response = await client.PostAsync(baseUrl,new StreamContent(audio));
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.Created)
                {
                        result.Success = true;
                        result.TokenResponse = token;
                        result.Response = true;
                   
                }
                else
                {
                    result.Success = false;
                    result.Message = "";
                }
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Message = e.Message;
            }
            return result;
        }



        /// <summary>
        /// Inserts the image.
        /// </summary>
        /// <returns>The image.</returns>
        /// <param name="image">Image.</param>
        /// <param name="path">Path.</param>
        /// <param name="imageName">Image name.</param>
        /// <param name="token">Token.</param>
        public async Task<ServiceResult> InsertImage(Stream image, string path, string imageName, string token)
        {
            string baseUrl = ConfigProperties.API_ENDPOINT + ConfigProperties.PATH_INSERT_IMAGE + "?filename=" + imageName;
            ServiceResult result = new ServiceResult();
            try
            {
                
                var response = await client.PostAsync(baseUrl, new StreamContent(image));
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.Created)
                {
                    result.Success = true;
                    result.TokenResponse = token;
                    result.Response = true;

                }
                else
                {
                    result.Success = false;
                    result.Message = "";
                }
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Message = e.Message;
            }
            return result;
        }



        /// <summary>
        /// Inserts the image.
        /// </summary>
        /// <returns>The image.</returns>
        /// <param name="image">Image.</param>
        /// <param name="path">Path.</param>
        /// <param name="imageName">Image name.</param>
        /// <param name="token">Token.</param>
        public async Task<ServiceResult> InsertImageByte(byte[] bitmapData, string path, string imageName, string token)
        {
            imageName = imageName.Contains(".jpg") ? imageName : "denunciaPhoto_"+Guid.NewGuid()+".jpg" ;
            string baseUrl = ConfigProperties.API_ENDPOINT + ConfigProperties.PATH_INSERT_IMAGE + "?filename=" + imageName;
            ServiceResult result = new ServiceResult();
            try
            {
                var fileContent = new ByteArrayContent(bitmapData);

                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "file",
                    FileName = imageName
                };
                /* string boundary = "---8d0f01e6b3b5dafaaadaad";
                MultipartFormDataContent multipartContent = new MultipartFormDataContent(boundary);
                multipartContent.Add(fileContent);*/
                var fileContentStream = await fileContent.ReadAsStreamAsync();
                var response = await client.PostAsync(baseUrl, new StreamContent(fileContentStream));
                client.Timeout = TimeSpan.FromSeconds(40);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.Created)
                {
                    result.Success = true;
                    result.TokenResponse = token;
                    result.Response = true;

                }
                else
                {
                    result.Success = false;
                    result.Message = "";
                }
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Message = e.Message;
            }
            result.Response = imageName;
            return result;
        }



        /// <summary>
        /// Gets the map geo.
        /// </summary>
        /// <returns>The map geo.</returns>
        /// <param name="lat">Lat.</param>
        /// <param name="lon">Lon.</param>
        public async Task<ServiceResult> GetMapGeo(string lat,string lon, string mapKey)
        {
            string baseUrl = ConfigProperties.GOOGLE_API_ENDPOINT + "latlng="+lat+","+lon+"&sensor=true&key="+mapKey;
            ServiceResult result = new ServiceResult();
            try
            {
                var response = await client.GetAsync(baseUrl);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    MapGeocode responseObj = JsonConvert.DeserializeObject<MapGeocode>(responseString);
                    result.Success = true;
                    result.Response = responseObj;
                }
                else
                {
                    result.Success = false;
                    result.Message = "";
                }
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Message = e.Message;
            }
            return result;
        }
    }
}
