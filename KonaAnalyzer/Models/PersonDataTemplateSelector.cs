using KonaAnalyzer.ViewModels;
using Xamarin.Forms;

namespace KonaAnalyzer.Models
{
    public class PersonDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Template1 { get; set; }
        public DataTemplate Template2 { get; set; }
        public bool UseSecondTemplate { get; set; }
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return ((StateControlViewModel)item).State == "Ohio" ? Template1 : Template2;
        }
    }
}