using Microsoft.AspNetCore.Mvc;

namespace FurnitureProject.Helper
{
    public static class LayoutHelper
    {
        public static void SetViewBagForLayout(Controller controller, bool useLayout = true, string layoutType = "user")
        {
            controller.ViewBag.UseLayout = useLayout;
            controller.ViewBag.LayoutType = layoutType;
        }
    }
}
