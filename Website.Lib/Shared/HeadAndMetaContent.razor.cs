using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Website.Lib;

/// <summary>
/// Page &lt;head&gt; content including meta data.
/// </summary>
public partial class HeadAndMetaContent : ComponentBase
{
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;


    /// <summary>
    /// The page description.
    /// </summary>
    [Parameter] public string Description { get; set; } = "";


    /// <summary>
    /// Optional open graph image.
    /// </summary>
    [Parameter] public string OpenGraphImage { get; set; } = "";


    /// <summary>
    /// Optional twitter image.
    /// </summary>
    [Parameter] public string TwitterImage { get; set; } = "";


    /// <summary>
    /// Page title.
    /// </summary>
    [Parameter] public string PageTitle { get; set; } = "";


    /// <summary>
    /// Optional child content.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;


    private RenderFragment Tags { get; set; } = default!;


    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        int sequence = 0;

        Tags = new(builder =>
        {
            sequence = BuildMetaTag(builder, sequence, "description", Description);
            sequence = BuildMetaTag(builder, sequence, "version", PackageInformation.Version);
            sequence = BuildMetaTag(builder, sequence, "build_time", PackageInformation.BuildTimeString);

            sequence = BuildMetaTag(builder, sequence, "og:title", PageTitle);
            sequence = BuildMetaTag(builder, sequence, "og:description", Description);
            sequence = BuildMetaTag(builder, sequence, "og:image", OpenGraphImage);
            sequence = BuildMetaTag(builder, sequence, "og:url", NavigationManager.Uri);
            sequence = BuildMetaTag(builder, sequence, "og:site_name", "Dioptra - real estate financial analytics");

            sequence = BuildMetaTag(builder, sequence, "twitter:card", "summary");
            sequence = BuildMetaTag(builder, sequence, "twitter:site", "@DioptraIO");
            sequence = BuildMetaTag(builder, sequence, "twitter:creator", "@SimonZieglerUK");
            sequence = BuildMetaTag(builder, sequence, "twitter:title", PageTitle);
            sequence = BuildMetaTag(builder, sequence, "twitter:description", Description);
            sequence = BuildMetaTag(builder, sequence, "twitter:image", TwitterImage);
        });


        int BuildMetaTag(RenderTreeBuilder builder, int sequence, string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                builder.OpenElement(sequence, "meta");
                builder.AddAttribute(sequence + 1, "name", name);
                builder.AddAttribute(sequence + 2, "content", value);
                builder.CloseElement();
            }

            return sequence + 3;
        }
    }
}
