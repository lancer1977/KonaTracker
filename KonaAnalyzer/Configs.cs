using Xamarin.Forms;

namespace KonaAnalyzer
{
    public class LabelClickBehavior : Behavior<Label>
    {
        protected override void OnAttachedTo(BindableObject bindable)
        {
            base.OnAttachedTo(bindable);

        }
    }
    public static class Configs
    {
        public static string AppCenterSecret = "__appCenterSecret__";
    }
}