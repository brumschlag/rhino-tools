//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.42
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Query {
    
    
    public partial class Where {
        
        /// Query for member _root_query_Cat
        static Root_Query_Cat _root_query_Cat = new Root_Query_Cat();
        
        /// Query for member _root_query_DomesticCat
        static Root_Query_DomesticCat _root_query_DomesticCat = new Root_Query_DomesticCat();
        
        /// Query for member Cat
        public static Root_Query_Cat Cat {
            get {
                return _root_query_Cat;
            }
        }
        
        /// Query for member DomesticCat
        public static Root_Query_DomesticCat DomesticCat {
            get {
                return _root_query_DomesticCat;
            }
        }
        
        /// Query for member Query_Cat
        public partial class Query_Cat<T1> : Query.QueryBuilder<T1>
         {
            
            /// Query for member .ctor
            public Query_Cat(string name, string associationPath) : 
                    base(name, associationPath) {
            }
            
            /// Query for member .ctor
            public Query_Cat(string name, string associationPath, bool backTrackAssociationOnEquality) : 
                    base(name, associationPath, backTrackAssociationOnEquality) {
            }
            
            /// Query for member 
            public virtual Query.PropertyQueryBuilder<T1> subclass {
                get {
                    string temp = associationPath;
                    return new Query.PropertyQueryBuilder<T1>("subclass", temp);
                }
            }
            
            /// Query for member 
            public virtual Query.QueryBuilder<T1> Id {
                get {
                    string temp = associationPath;
                    return new Query.QueryBuilder<T1>("Id", temp);
                }
            }
        }
        
        /// Query for member Root_Query_Cat
        public partial class Root_Query_Cat : Query_Cat<NHibernate.Query.Generator.Tests.ActiveRecord.Cat> {
            
            /// Query for member .ctor
            public Root_Query_Cat() : 
                    base("this", null) {
            }
        }
        
        /// Query for member Query_DomesticCat
        public partial class Query_DomesticCat<T2> : Query_Cat<T2>
         {
            
            /// Query for member .ctor
            public Query_DomesticCat(string name, string associationPath) : 
                    base(name, associationPath) {
            }
            
            /// Query for member .ctor
            public Query_DomesticCat(string name, string associationPath, bool backTrackAssociationOnEquality) : 
                    base(name, associationPath, backTrackAssociationOnEquality) {
            }
            
            /// Query for member 
            public virtual Query.PropertyQueryBuilder<T2> Name {
                get {
                    string temp = associationPath;
                    return new Query.PropertyQueryBuilder<T2>("Name", temp);
                }
            }
        }
        
        /// Query for member Root_Query_DomesticCat
        public partial class Root_Query_DomesticCat : Query_DomesticCat<NHibernate.Query.Generator.Tests.ActiveRecord.DomesticCat> {
            
            /// Query for member .ctor
            public Root_Query_DomesticCat() : 
                    base("this", null) {
            }
        }
    }
    
    public partial class OrderBy {
        
        /// Query for member Cat
        public partial class Cat {
            
            /// Query for member subclass
            public static Query.OrderByClause subclass {
                get {
                    return new Query.OrderByClause("subclass");
                }
            }
            
            /// Query for member Id
            public static Query.OrderByClause Id {
                get {
                    return new Query.OrderByClause("Id");
                }
            }
        }
        
        /// Query for member DomesticCat
        public partial class DomesticCat : Cat {
            
            /// Query for member Name
            public static Query.OrderByClause Name {
                get {
                    return new Query.OrderByClause("Name");
                }
            }
        }
    }
    
    public partial class ProjectBy {
        
        /// Query for member Cat
        public partial class Cat {
            
            /// Query for member subclass
            public static Query.PropertyProjectionBuilder subclass {
                get {
                    return new Query.PropertyProjectionBuilder("subclass");
                }
            }
        }
        
        /// Query for member DomesticCat
        public partial class DomesticCat : Cat {
            
            /// Query for member Name
            public static Query.PropertyProjectionBuilder Name {
                get {
                    return new Query.PropertyProjectionBuilder("Name");
                }
            }
        }
    }
    
    public partial class GroupBy {
        
        /// Query for member Cat
        public partial class Cat {
            
            /// Query for member subclass
            public static NHibernate.Expression.IProjection subclass {
                get {
                    return NHibernate.Expression.Projections.GroupProperty("subclass");
                }
            }
        }
        
        /// Query for member DomesticCat
        public partial class DomesticCat : Cat {
            
            /// Query for member Name
            public static NHibernate.Expression.IProjection Name {
                get {
                    return NHibernate.Expression.Projections.GroupProperty("Name");
                }
            }
        }
    }
}