using Microsoft.AspNetCore.Razor.TagHelpers;

namespace LanchesMac.TagHelpers
{
    public class EmailTagHelper : TagHelper
    {
        public string Endereco { get; set; }
        public string Conteudo { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a"; // Define a tag como <a>
            output.Attributes.SetAttribute("href", "mailto:" + Endereco); // Define o atributo href com o endereço de email
            output.Content.SetContent(Conteudo); // Define o conteúdo da tag
        }
    }
}
