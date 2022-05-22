namespace Website.Lib;


using Microsoft.AspNetCore.Components;
using System;


/// <summary>
/// Displays an SVG Dioptra logo
/// </summary>
public partial class DioptraLogo
{
    /// <summary>
    /// The logo's foreground color.
    /// </summary>
    [Parameter] public string Color { get; set; } = "var(--mdc-theme-on-surface)";


    /// <summary>
    /// Height, defaults to 252px.
    /// </summary>
    [Parameter] public string Height { get; set; } = DefaultHeight.ToString();


    /// <summary>
    /// The suffix for the logo's CSS class name: "logo-[ClassSuffix]". Defaults to a GUID.
    /// </summary>
    [Parameter] public string ClassSuffix { get; set; } = "";


    /// <summary>
    /// Extra CSS classes to add to the logo.
    /// </summary>
    [Parameter] public string ExtraClasses { get; set; } = "";


    /// <summary>
    /// Standard deviation for a drop shadow. Defaults to zero/no drop shadow.
    /// </summary>
    [Parameter] public double DropShadowStdDev { get; set; } = 0;



    private static readonly double DefaultHeight = 31.75;
    private static readonly double DefaultWidth = 31.75;
    private readonly string GuidId = Guid.NewGuid().ToString();


    private string PathId => "path-" + GuidId;
    private string FilterId => "dropshadow-" + GuidId;
    private bool HasDropShadow => DropShadowStdDev > 0.01;
    private string ClassName => "logo-" + (string.IsNullOrWhiteSpace(ClassSuffix) ? GuidId : ClassSuffix);
    private string FilterStyle => HasDropShadow ? $"filter: url(#{FilterId});" : "";
    private string FillStyle => $"fill: {Color};";
    private string CompoundStyle => $"{FillStyle} {FilterStyle}";
    private double CalcHeight => Convert.ToDouble(Height);
    private double CalcWidth => DefaultWidth * Scale;
    private double Scale => CalcHeight / DefaultHeight;
    private double HeightRounded => Math.Round(CalcHeight, 2);
    private double WidthRounded => Math.Round(CalcWidth, 2);
    private double ScaleRounded => Math.Round(Scale, 2);
    private double CalcDropShadowStdDev => Math.Round(DropShadowStdDev / Scale, 2);
}
