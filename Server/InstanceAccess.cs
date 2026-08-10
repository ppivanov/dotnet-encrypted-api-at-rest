using Microsoft.AspNetCore.Mvc;

namespace EncryptedDbAtRest.Server;

public static class InstanceAccess
{

    public static Instance? Get([FromServices] DbContext dbContext)
    {
        return dbContext.Customers.FirstOrDefault();
    }

    public static object Create([FromServices] DbContext dbContext, HttpContext httpContext)
    {
        dbContext.Customers.Add(new Customer(new("asd", "asdsdgf"), "a", "b", "c", "d"));
        dbContext.SaveChanges();

        return dbContext.Customers.FirstOrDefault().Deserialize();
    }
}