//------------------------------------------------------------------------------
/// <autogenerated>
///     This code was generated by a tool.
///     Runtime Version: 1.1.4322.573
///
///     Changes to this file may cause incorrect behavior and will be lost if 
///     the code is regenerated.
/// </autogenerated>
//------------------------------------------------------------------------------

//@cc_on
//@set @debug(off)


package MyNameSpace
{
    
    public class BaseType
    {
        
        public function BaseType()
        {
            
        }
        
        public function BaseType(TestStringParameter : System.String)
        {
            
        }
    }
    
    public class DerivedType extends BaseType
    {
        
        public function DerivedType(System.CodeDom.CodeArgumentReferenceExpression : System.String)
        {
            super(TestStringParameter);
            
        }
        
        public function DerivedType()
        {
            super("test");
            
        }
        
        public function DerivedType(TestIntParameter : int)
        {
            this();
            
        }
    }
}
