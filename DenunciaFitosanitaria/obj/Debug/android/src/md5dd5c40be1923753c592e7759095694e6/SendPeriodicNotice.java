package md5dd5c40be1923753c592e7759095694e6;


public class SendPeriodicNotice
	extends mono.android.app.IntentService
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onBind:(Landroid/content/Intent;)Landroid/os/IBinder;:GetOnBind_Landroid_content_Intent_Handler\n" +
			"n_onStartCommand:(Landroid/content/Intent;II)I:GetOnStartCommand_Landroid_content_Intent_IIHandler\n" +
			"n_onCreate:()V:GetOnCreateHandler\n" +
			"n_onHandleIntent:(Landroid/content/Intent;)V:GetOnHandleIntent_Landroid_content_Intent_Handler\n" +
			"";
		mono.android.Runtime.register ("DenunciaFitosanitaria.Services.SendPeriodicNotice, DenunciaFitosanitaria", SendPeriodicNotice.class, __md_methods);
	}


	public SendPeriodicNotice (java.lang.String p0)
	{
		super (p0);
		if (getClass () == SendPeriodicNotice.class)
			mono.android.TypeManager.Activate ("DenunciaFitosanitaria.Services.SendPeriodicNotice, DenunciaFitosanitaria", "System.String, mscorlib", this, new java.lang.Object[] { p0 });
	}


	public SendPeriodicNotice ()
	{
		super ();
		if (getClass () == SendPeriodicNotice.class)
			mono.android.TypeManager.Activate ("DenunciaFitosanitaria.Services.SendPeriodicNotice, DenunciaFitosanitaria", "", this, new java.lang.Object[] {  });
	}


	public android.os.IBinder onBind (android.content.Intent p0)
	{
		return n_onBind (p0);
	}

	private native android.os.IBinder n_onBind (android.content.Intent p0);


	public int onStartCommand (android.content.Intent p0, int p1, int p2)
	{
		return n_onStartCommand (p0, p1, p2);
	}

	private native int n_onStartCommand (android.content.Intent p0, int p1, int p2);


	public void onCreate ()
	{
		n_onCreate ();
	}

	private native void n_onCreate ();


	public void onHandleIntent (android.content.Intent p0)
	{
		n_onHandleIntent (p0);
	}

	private native void n_onHandleIntent (android.content.Intent p0);

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
