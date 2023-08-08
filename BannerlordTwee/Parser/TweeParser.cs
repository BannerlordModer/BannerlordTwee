using BannerlordTwee.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BannerlordTwee.Parser {
    public class TweeParser {
        private readonly string TweeData;
        public TweeParser(string TweeData) {
            this.TweeData = TweeData;
        }
        public Story ParseToStory() {

            return new Story();
        }
    }
}
