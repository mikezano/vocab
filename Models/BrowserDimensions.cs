using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocab.Models
{
    public class BrowserDimensions
    {
        private static int CARD_HEIGHT = 208;
        private static int CARD_WIDTH = 300;
        private static int TITLE_HEIGHT = 57;
        private static int FOOTER_HEIGHT = 50;

        public int Width { get; set; }
        public int Height { get; set; }

        public int GetVisibleCardCount()
        {
            int horizontalCount = Width / CARD_WIDTH;
            int verticalCount = (Height - TITLE_HEIGHT - FOOTER_HEIGHT) / CARD_HEIGHT;

            return horizontalCount * verticalCount;
        }

    }
}
