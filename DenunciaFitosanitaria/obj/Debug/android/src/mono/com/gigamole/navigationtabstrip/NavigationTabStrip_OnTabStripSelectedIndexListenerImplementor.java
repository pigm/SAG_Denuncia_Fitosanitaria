package mono.com.gigamole.navigationtabstrip;


public class NavigationTabStrip_OnTabStripSelectedIndexListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.gigamole.navigationtabstrip.NavigationTabStrip.OnTabStripSelectedIndexListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onEndTabSelected:(Ljava/lang/String;I)V:GetOnEndTabSelected_Ljava_lang_String_IHandler:Com.Gigamole.Navigationtabstrip.NavigationTabStrip/IOnTabStripSelectedIndexListenerInvoker, NavigationTabStrip\n" +
			"n_onStartTabSelected:(Ljava/lang/String;I)V:GetOnStartTabSelected_Ljava_lang_String_IHandler:Com.Gigamole.Navigationtabstrip.NavigationTabStrip/IOnTabStripSelectedIndexListenerInvoker, NavigationTabStrip\n" +
			"";
		mono.android.Runtime.register ("Com.Gigamole.Navigationtabstrip.NavigationTabStrip+IOnTabStripSelectedIndexListenerImplementor, NavigationTabStrip", NavigationTabStrip_OnTabStripSelectedIndexListenerImplementor.class, __md_methods);
	}


	public NavigationTabStrip_OnTabStripSelectedIndexListenerImplementor ()
	{
		super ();
		if (getClass () == NavigationTabStrip_OnTabStripSelectedIndexListenerImplementor.class)
			mono.android.TypeManager.Activate ("Com.Gigamole.Navigationtabstrip.NavigationTabStrip+IOnTabStripSelectedIndexListenerImplementor, NavigationTabStrip", "", this, new java.lang.Object[] {  });
	}


	public void onEndTabSelected (java.lang.String p0, int p1)
	{
		n_onEndTabSelected (p0, p1);
	}

	private native void n_onEndTabSelected (java.lang.String p0, int p1);


	public void onStartTabSelected (java.lang.String p0, int p1)
	{
		n_onStartTabSelected (p0, p1);
	}

	private native void n_onStartTabSelected (java.lang.String p0, int p1);

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
