using System;
using System.Collections.Generic;
using english.DbFolder;
namespace english.ViewModel.AdminViewModel
{
    public class UsersView
    {
        public List<User> Users { get; set; }
        public Pager Pager { get; set; }

        public UsersView()
        {
            Users = new List<User>();
        }
    }
    public class Pager
    {
        public Pager(int totalItems, int? page, int pageSize = 5)
        {
            // calculate total, start and end pages
            var totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);
            var currentPage = page != null ? (int)page : 1;
            var startPage = currentPage -6;
            var endPage = currentPage + 5;
            if (startPage <= 0)
            {
                endPage -= (startPage - 1);
                startPage = 1;
            }
            if (endPage > totalPages)
            {
                endPage = totalPages;
                if (endPage > pageSize)
                {
                    startPage = endPage - pageSize - 1;
                }
            }

            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;
        }
        public int TotalItems { get; private set; }
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set; }
    }
}
