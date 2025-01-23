using Sirenix.OdinInspector;
using System;
using System.Diagnostics;

namespace EasyFramework
{
[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
[Conditional("UNITY_EDITOR")]
public sealed class BigTitleGroupCNAttribute : PropertyGroupAttribute
{
	/// <summary>
	/// Optional subtitle.
	/// </summary>
	public string Subtitle;

	/// <summary>
	/// Title alignment.
	/// </summary>
	public TitleAlignments Alignment;

	/// <summary>
	/// Gets a value indicating whether or not to draw a horizontal line below the title.
	/// </summary>
	public bool HorizontalLine;

	/// <summary>
	/// If <c>true</c> the title will be displayed with a bold font.
	/// </summary>
	public bool BoldTitle;

	/// <summary>
	/// Gets a value indicating whether or not to indent all group members.
	/// </summary>
	public bool Indent;

	/// <summary>
	/// Groups properties vertically together with a title, an optional subtitle, and an optional horizontal line. 
	/// </summary>
	/// <param name="title">The title-</param>
	/// <param name="subtitle">Optional subtitle.</param>
	/// <param name="alignment">The text alignment.</param>
	/// <param name="horizontalLine">Horizontal line.</param>
	/// <param name="boldTitle">Bold text.</param>
	/// <param name="indent">Whether or not to indent all group members.</param>
	/// <param name="order">The group order.</param>
	public BigTitleGroupCNAttribute(string title, string subtitle = null, TitleAlignments alignment = TitleAlignments.Left, bool horizontalLine = true, bool boldTitle = true, bool indent = false, float order = 0f)
		: base(title, order)
	{
		Subtitle = subtitle;
		Alignment = alignment;
		HorizontalLine = horizontalLine;
		BoldTitle = boldTitle;
		Indent = indent;
	}

	/// <summary>
	/// Combines TitleGroup attributes.
	/// </summary>
	/// <param name="other">The other group attribute to combine with.</param>
	protected override void CombineValuesWith(PropertyGroupAttribute other)
	{
		TitleGroupAttribute t = other as TitleGroupAttribute;
		if (Subtitle != null)
		{
			t.Subtitle = Subtitle;
		}
		else
		{
			Subtitle = t.Subtitle;
		}
		if (Alignment != 0)
		{
			t.Alignment = Alignment;
		}
		else
		{
			Alignment = t.Alignment;
		}
		if (!HorizontalLine)
		{
			t.HorizontalLine = HorizontalLine;
		}
		else
		{
			HorizontalLine = t.HorizontalLine;
		}
		if (!BoldTitle)
		{
			t.BoldTitle = BoldTitle;
		}
		else
		{
			BoldTitle = t.BoldTitle;
		}
		if (Indent)
		{
			t.Indent = Indent;
		}
		else
		{
			Indent = t.Indent;
		}
	}
}
}
