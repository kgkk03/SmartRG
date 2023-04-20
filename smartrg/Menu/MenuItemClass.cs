using System;
using Xamarin.Forms;

namespace smartrg.Menu
{
    public class MenuItemClass
    {
        public MenuItemClass()
        {
            TargetType = typeof(MenuItemClass);
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public ImageSource Image { get; set; }
        public Type TargetType { get; set; }
        public bool IsVisible { get; set; } = false;


    }
}
