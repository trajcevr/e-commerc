using Core.Entites;

namespace Core.Specifications;

public class TypeListSpecification : BaseSpecification<Product, string>
{
    public TypeListSpecification()
    {
        AddSelect(x => x.Type);  
        ApplyDisctinct();  
    }
}
