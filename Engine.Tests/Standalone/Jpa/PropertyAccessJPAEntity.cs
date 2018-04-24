namespace Engine.Tests.Standalone.Jpa
{
    /// <summary>
    ///     Simple JPA entity, id is set on a property.
    /// </summary>
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Entity(name = "JPA_ENTITY_P") public class PropertyAccessJPAEntity
    public class PropertyAccessJPAEntity
    {
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Id @Column(name = "ID_") public Nullable<long> getId()
        public virtual long? Id { get; set; }


        public virtual string Value { get; set; }
    }
}