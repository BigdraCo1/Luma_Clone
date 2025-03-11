using alma.Models;
using alma.Services;

namespace alma.Utils;

public class DatabaseInitilaizer {
    public static void Seed(IApplicationBuilder applicationBuilder) {
        using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();
        var context = serviceScope.ServiceProvider.GetService<DatabaseContext>();
        var env = serviceScope.ServiceProvider.GetService<IWebHostEnvironment>();

        if (context is null) {
            throw new Exception("DatabaseContext is null");
        }

        if (env is null) {
            throw new Exception("IWebHostEnvironment is null");
        }

        if (context.Tag.Any()) {
            return;
        }

        var tags = new List<Tag> {
                new() {Id = Tuid.Generate(), NameEN = "Social", NameTH = "สังคม", DescriptionEN = "Casual get-togethers, parties, celebrations, and social events", DescriptionTH="งานเลี้ยง, งานเฉลิมฉลอง, และงานสังสรรค์", Icon = "party-popper", Image = File.ReadAllBytes(Path.Combine(env.WebRootPath, "images", "tags", "social.avif")), ImageType = "image/avif"},
                new() {Id = Tuid.Generate(), NameEN = "Study", NameTH = "การเรียน", DescriptionEN = "Skill development, education, and learning", DescriptionTH = "การพัฒนาทักษะ, การศึกษา, และการเรียนรู้", Icon = "notebook-pen", Image = File.ReadAllBytes(Path.Combine(env.WebRootPath, "images", "tags", "study.avif")), ImageType = "image/avif"},
                new() {Id = Tuid.Generate(), NameEN = "Performances", NameTH = "การแสดง", DescriptionEN = "Live music events, concerts, performances, shows, and entertainment", DescriptionTH = "งานดนตรี, คอนเสิร์ต, การแสดง, โชว์, และสื่อความบันเทิง", Icon = "guitar", Image = File.ReadAllBytes(Path.Combine(env.WebRootPath, "images", "tags", "performances.avif")), ImageType = "image/avif"},
                new() {Id = Tuid.Generate(), NameEN = "Wellbeing", NameTH = "สุขภาพ", DescriptionEN = "Physical activity, health, and wellness", DescriptionTH = "การออกกำลังกาย, สุขภาพ, และความเป็นอยู่ที่ดี", Icon = "dumbbell", Image = File.ReadAllBytes(Path.Combine(env.WebRootPath, "images", "tags", "wellbeing.avif")), ImageType = "image/avif"},
                new() {Id = Tuid.Generate(), NameEN = "Food & Drink", NameTH = "อาหารและเครื่องดื่ม", DescriptionEN = "Food, beverages, dining, and culinary experiences", DescriptionTH = "งานอาหาร, เครื่องดื่ม, ร้านอาหาร, และประสบการณ์ทางครัว", Icon = "pizza", Image = File.ReadAllBytes(Path.Combine(env.WebRootPath, "images", "tags", "food.avif")), ImageType = "image/avif"},
                new() {Id = Tuid.Generate(), NameEN = "Business", NameTH = "ธุรกิจ", DescriptionEN = "Networking, entrepreneurship, business, and professional development", DescriptionTH = "การสร้างเครือข่าย, การประกอบการ, ธุรกิจ, และการพัฒนาอาชีพ", Icon = "handshake", Image = File.ReadAllBytes(Path.Combine(env.WebRootPath, "images", "tags", "business.avif")), ImageType = "image/avif"},
                new() {Id = Tuid.Generate(), NameEN = "Community & Causes", NameTH = "ชุมชน", DescriptionEN = "Volunteering, charity, community service, and social causes", DescriptionTH = "การอาสาสมัคร, กิจกรรมกุศล, การบริการชุมชน, และกิจกรรมเพื่อสังคม", Icon = "trees", Image = File.ReadAllBytes(Path.Combine(env.WebRootPath, "images", "tags", "community.avif")), ImageType = "image/avif"},
                new() {Id = Tuid.Generate(), NameEN = "Others", NameTH = "อื่นๆ", DescriptionEN = "Events that do not fit into any other category", DescriptionTH = "งานที่ไม่ตรงกับหมวดหมู่ใดๆ", Icon = "puzzle", Image = File.ReadAllBytes(Path.Combine(env.WebRootPath, "images", "tags", "others.avif")), ImageType = "image/avif"}
            };

        context.Tag.AddRange(tags);
        context.SaveChanges();
    }
}