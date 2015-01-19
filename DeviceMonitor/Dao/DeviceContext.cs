using System.Data.Entity;
using System.Web;
using DeviceMonitor.Models.DeviceModels;

namespace DeviceMonitor.Dao
{

    public class DeviceContext : DbContext
    {
        public DeviceContext()
            : base("DefaultConnection")
        {
            //Configuration.ProxyCreationEnabled = false;
        }
        public static DeviceContext Instance
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return null;
                }
                if (HttpContext.Current.Items["DeviceContext "] == null)
                {
                    HttpContext.Current.Items["DeviceContext "] = new DeviceContext();
                }
                return HttpContext.Current.Items["DeviceContext "] as DeviceContext;
            }
            set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.Cache["DeviceContext "] = value;
            }
        }
        public DbSet<DeviceGroup> DeviceGroups { get; set; }
        public DbSet<DeviceInfo> DeviceInfos { get; set; }
        public DbSet<DeviceData> DeviceDatas { get; set; }
        public DbSet<DeviceDataFormat> DeviceDataFormats { get; set; }
        public DbSet<DeviceFactory> DeviceFactories { get; set; }
    }
}