using System;

namespace MessageBoard.Models
{
  public interface IUriService
  {
      public Uri GetPageUri(PaginationFilter filter, string route);
  }
}