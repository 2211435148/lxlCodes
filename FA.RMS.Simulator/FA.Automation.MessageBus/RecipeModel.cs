using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FA.Automation.MessageBus
{
    public class RecipeModel
    {
        public string RecipeId { get; set; }
        public RecipeCategroyEnum RecipeCategory { get; set; } = RecipeCategroyEnum.Sub;
        public string RecipeFileName { get; set; }
    }

    public enum RecipeCategroyEnum
    {
        Main,
        Sub
    }
}
