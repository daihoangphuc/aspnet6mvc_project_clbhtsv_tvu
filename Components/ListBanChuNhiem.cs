using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using website_CLB_HTSV.Data;
using website_CLB_HTSV.Models;

namespace website_CLB_HTSV.Components
{
    public class ListBanChuNhiem : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public ListBanChuNhiem(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var applicationDbContext = _context.SinhVien.Include(s => s.ChucVu).Include(s => s.LopHoc).Where(s => s.ChucVu.MaChucVu != "CV04" && s.ChucVu.MaChucVu != null);

            // Đếm tổng số sinh viên
            int totalSinhViens = await _context.SinhVien.CountAsync();
            ViewBag.TotalSinhViens = totalSinhViens;

            return View("Index" , await applicationDbContext.ToListAsync());
        }
    }
}
