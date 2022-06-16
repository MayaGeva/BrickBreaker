package crc64eba02ca91e4aa0ef;


public class CountDown
	extends android.view.SurfaceView
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("BrickBreaker.CountDown, BrickBreaker", CountDown.class, __md_methods);
	}


	public CountDown (android.content.Context p0)
	{
		super (p0);
		if (getClass () == CountDown.class)
			mono.android.TypeManager.Activate ("BrickBreaker.CountDown, BrickBreaker", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public CountDown (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == CountDown.class)
			mono.android.TypeManager.Activate ("BrickBreaker.CountDown, BrickBreaker", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public CountDown (android.content.Context p0, android.util.AttributeSet p1, int p2)
	{
		super (p0, p1, p2);
		if (getClass () == CountDown.class)
			mono.android.TypeManager.Activate ("BrickBreaker.CountDown, BrickBreaker", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public CountDown (android.content.Context p0, android.util.AttributeSet p1, int p2, int p3)
	{
		super (p0, p1, p2, p3);
		if (getClass () == CountDown.class)
			mono.android.TypeManager.Activate ("BrickBreaker.CountDown, BrickBreaker", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2, p3 });
	}

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
