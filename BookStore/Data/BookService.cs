using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Data
{
    
    public class BookService
    {
        private AppDataContext appDataContext;

        public BookService(AppDataContext appDataContext)
        {
            this.appDataContext = appDataContext;
        }

        public ICollection<Book> ListAllBooks()
        {
            return appDataContext.Book.ToList();
        }

        public List<Category> GetAllCategories()
        {
            return appDataContext.Category.ToList();
        }
        /// <summary>
        /// lấy ra all danh sách Book
        /// </summary>
        /// <returns></returns>
        public List<Book> GetAllBookOfCategory(int categoryId)
        {
            var query = from book in appDataContext.Book
                        join category_book in appDataContext.Categorybook on book.Id equals category_book.BookId
                        join category in appDataContext.Category on category_book.CategoryId equals category.Id
                        where category_book.CategoryId == categoryId
                        select book;
            return query.ToList();
        }
        /// <summary>
        /// Lấy ra thông tin của 1 book 
        /// </summary>
        /// <param name="id">mã book</param>
        /// <returns>Book</returns>
        public Book GetById(int id)
        {
            return appDataContext.Book.Find(id);
        }
        /// <summary>
        /// Lấy ra danh sách thông tin của book và số lượng đặt
        /// </summary>
        /// <param name="bookIds">Dictionary chưa key là mã book value là số lượng đặt</param>
        /// <returns>Dictionary<Book, int></returns>
        public Dictionary<Book, int> FindAll(Dictionary<int, int> bookIds)
        {
            var query = from key_Value in bookIds
                        select new KeyValuePair<Book, int>(appDataContext.Book.Find(key_Value.Key), key_Value.Value);
            return query.ToDictionary(v => v.Key, v => v.Value);
        }
        /// <summary>
        /// Lấy danh dánh category và số lượng sách của nó
        /// </summary>
        /// <returns>Dictionary<Category, int></returns>
        public Dictionary<Category, int> GetAllCategoryCount()
        {
            var query = from category in appDataContext.Category
                    select new KeyValuePair<Category,int>(category, (from category_book in appDataContext.Categorybook where category_book.CategoryId == category.Id 
                                                             select category_book).Count());

            return query.ToDictionary(v => v.Key, v => v.Value);
        }
        /// <summary>
        /// Phân trang
        /// </summary>
        /// <param name="categoryId">Mã Category</param>
        /// <param name="tolalRecord">tổng số record của all list book</param>
        /// <param name="pageIndex">trang gửi lên</param>
        /// <param name="pageSize">số lượng book trong 1 trang</param>
        /// <returns>danh sách book</returns>
        public List<Book> GetAllBookOfCategory(int categoryId,ref int tolalRecord,int pageIndex =1,int pageSize=6)
        {
            tolalRecord = (from book in appDataContext.Book
                           join category_book in appDataContext.Categorybook on book.Id equals category_book.BookId
                           join category in appDataContext.Category on category_book.CategoryId equals category.Id
                           where category_book.CategoryId == categoryId
                           select book).Count();
            var query = (from book in appDataContext.Book
                        join category_book in appDataContext.Categorybook on book.Id equals category_book.BookId
                        join category in appDataContext.Category on category_book.CategoryId equals category.Id
                        where category_book.CategoryId == categoryId
                        select book).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            return query.ToList();
        }
    }
}
