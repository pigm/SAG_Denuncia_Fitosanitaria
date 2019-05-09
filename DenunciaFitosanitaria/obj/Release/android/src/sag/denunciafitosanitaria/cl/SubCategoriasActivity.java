package sag.denunciafitosanitaria.cl;


public class SubCategoriasActivity
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
			"n_onResume:()V:GetOnResumeHandler\n" +
			"";
		mono.android.Runtime.register ("DenunciaFitosanitaria.Activities.SubCategoriasActivity, DenunciaFitosanitaria", SubCategoriasActivity.class, __md_methods);
	}


	public SubCategoriasActivity ()
	{
		super ();
		if (getClass () == SubCategoriasActivity.class)
			mono.android.TypeManager.Activate ("DenunciaFitosanitaria.Activities.SubCategoriasActivity, DenunciaFitosanitaria", "", this, new java.lang.Object[] {  });
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
