﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using IronBoard.Core.Model;
using SharpSvn;

namespace IronBoard.Core.Presenters
{
   class PostCommitReviewPresenter
   {
      public DirectoryInfo LocalDirectory { get; set; }

      public IEnumerable<WorkItem> GetCommitedWorkItems(int maxRevisions)
      {
         var args = new SvnLogArgs {Limit = maxRevisions};

         using (var client = new SvnClient())
         {
            Collection<SvnLogEventArgs> entries;
            client.GetLog(LocalDirectory.FullName, args, out entries);

            if(entries != null && entries.Count > 0)
            {
               return entries.Select(e => new WorkItem(
                                             e.Revision.ToString(CultureInfo.InvariantCulture),
                                             e.Author,
                                             e.LogMessage,
                                             e.Time));
            }
         }

         return null;
      }

      public string ProduceDescription(IEnumerable<WorkItem> selectedItems)
      {
         if (selectedItems != null)
         {
            var b = new StringBuilder();
            foreach (WorkItem wi in selectedItems)
            {
               if (!string.IsNullOrEmpty(wi.Comment))
               {
                  //b.Append("--------- r");
                  //b.Append(wi.ItemId);
                  //b.Append("-----------");
                  //b.AppendLine();

                  b.AppendLine(wi.Comment);

               }
            }
            return b.ToString();
         }
         return null;
      }

      public string ToListString(WorkItem i)
      {
         return string.Format("{0}: {1}@{2}| {3}",
                              i.ItemId, i.Author, i.Time, i.Comment);
      }

      public Tuple<int, int> GetRange(IEnumerable<WorkItem> items)
      {
         int min = int.MaxValue, max = 0;
         if (items != null)
         {
            foreach (WorkItem wi in items)
            {
               int rev = int.Parse(wi.ItemId);
               if (rev < min) min = rev;
               if (rev > max) max = rev;
            }
         }
         return min <= max ? new Tuple<int, int>(min - 1, max) : null;
      }

      private string GetDiff(long fromRev, long toRev)
      {
         string diffText;
         using (var client = new SvnClient())
         {
            using (var ms = new MemoryStream())
            {
               client.Diff(
                  new SvnPathTarget(LocalDirectory.FullName),
                  new SvnRevisionRange(fromRev, toRev),
                  ms);

               ms.Position = 0;
               diffText = Encoding.UTF8.GetString(ms.ToArray());
            }
         }

         return diffText;
      }

      public void PostReview(long fromRev, long toRev,
         string summary, string description, string testing)
      {
         string diff = GetDiff(fromRev, toRev);
      }
   }
}
