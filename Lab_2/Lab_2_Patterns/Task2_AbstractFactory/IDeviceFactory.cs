namespace Lab2_Patterns.Task2_AbstractFactory
{
    public interface IDeviceFactory
    {
        Laptop CreateLaptop();
        Netbook CreateNetbook();
        EBook CreateEBook();
        Smartphone CreateSmartphone();
        string GetBrandName();
    }
}