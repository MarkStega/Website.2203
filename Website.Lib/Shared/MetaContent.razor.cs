using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Website.Lib.Shared;
public partial class MetaContent : ComponentBase
{
    [Inject] private NavigationManager NavigationManager { get; set; }


    [Parameter] public string Description { get; set; } = "";
    [Parameter] public string OpenGraphImage { get; set; } = "";
    [Parameter] public string TwitterImage { get; set; } = "";
    [Parameter] public string Title { get; set; } = "";


    private RenderFragment Tags { get; set; }


    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        int sequence = 0;

        Tags = new(builder =>
        {
            sequence = BuildMetaTag(builder, sequence, "description", Description);

            sequence = BuildMetaTag(builder, sequence, "og:title", Title);
            sequence = BuildMetaTag(builder, sequence, "og:description", Description);
            sequence = BuildMetaTag(builder, sequence, "og:image", OpenGraphImage);
            sequence = BuildMetaTag(builder, sequence, "og:url", NavigationManager.Uri);
            sequence = BuildMetaTag(builder, sequence, "og:site_name", "Dioptra - real estate financial analytics");

            sequence = BuildMetaTag(builder, sequence, "twitter:card", "summary");
            sequence = BuildMetaTag(builder, sequence, "twitter:site", "@DioptraIO");
            sequence = BuildMetaTag(builder, sequence, "twitter:creator", "@SimonZieglerUK");
            sequence = BuildMetaTag(builder, sequence, "twitter:title", Title);
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
