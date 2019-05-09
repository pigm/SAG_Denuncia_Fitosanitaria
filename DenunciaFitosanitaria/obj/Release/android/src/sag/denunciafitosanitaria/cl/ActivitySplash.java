package sag.denunciafitosanitaria.cl;


public class ActivitySplash
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"n_onResume:()V:GetOnResumeHandler\n" +
			"";
		mono.android.Runtime.register ("DenunciaFitosanitaria.Activities.ActivitySplash, DenunciaFitosanitaria", ActivitySplash.class, __md_methods);
	}


	public ActivitySplash ()
	{
		super ();
		if (getClass () == ActivitySplash.class)
			mono.android.TypeManager.Activate ("DenunciaFitosanitaria.Activities.ActivitySplash, DenunciaFitosanitaria", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);


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
