﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronBoard.Core.Views
{
   public interface IPostCommitReviewView
   {
      ILoginPasswordView CreateLoginPasswordView();
   }
}
