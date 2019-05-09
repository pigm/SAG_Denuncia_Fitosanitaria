package sag.denunciafitosanitaria.cl;


public class SubCategoriaDetalleActivity
	extends android.support.v7.app.AppCompatActivity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"n_onBackPressed:()V:GetOnBackPressedHandler\n" +
			"n_onSupportNavigateUp:()Z:GetOnSupportNavigateUpHandler\n" +
			"n_onActivityResult:(IILandroid/content/Intent;)V:GetOnActivityResult_IILandroid_content_Intent_Handler\n" +
			"n_onResume:()V:GetOnResumeHandler\n" +
			"";
		mono.android.Runtime.register ("DenunciaFitosanitaria.Activities.SubCategoriaDetalleActivity, DenunciaFitosanitaria", SubCategoriaDetalleActivity.class, __md_methods);
	}


	public SubCategoriaDetalleActivity ()
	{
		super ();
		if (getClass () == SubCategoriaDetalleActivity.class)
			mono.android.TypeManager.Activate ("DenunciaFitosanitaria.Activities.SubCategoriaDetalleActivity, DenunciaFitosanitaria", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);


	public void onBackPressed ()
	{
		n_onBackPressed ();
	}

	private native void n_onBackPressed ();


	public boolean onSupportNavigateUp ()
	{
		return n_onSupportNavigateUp ();
	}

	private native boolean n_onSupportNavigateUp ();


	public void onActivityResult (int p0, int p1, android.content.Intent p2)
	{
		n_onActivityResult (p0, p1, p2);
	}

	private native void n_onActivityResult (int p0, int p1, android.content.Intent p2);


	public void onResume ()
	{
		n_onResume ();
	}

	private native void n_onResume ();

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
