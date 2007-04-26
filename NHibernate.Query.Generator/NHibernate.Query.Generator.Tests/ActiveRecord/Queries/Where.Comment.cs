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
        
        /// Query for member _root_query_Comment
        static Root_Query_Comment _root_query_Comment = new Root_Query_Comment();
        
        /// Query for member Comment
        public static Root_Query_Comment Comment {
            get {
                return _root_query_Comment;
            }
        }
        
        /// Query for member Query_Comment
        public partial class Query_Comment<T1> : Query.QueryBuilder<T1>
         {
            
            /// Query for member .ctor
            public Query_Comment(string name, string associationPath) : 
                    base(name, associationPath) {
            }
            
            /// Query for member .ctor
            public Query_Comment(string name, string associationPath, bool backTrackAssociationOnEquality) : 
                    base(name, associationPath, backTrackAssociationOnEquality) {
            }
            
            /// Query for member 
            public virtual Query.PropertyQueryBuilder<T1> Author {
                get {
                    string temp = associationPath;
                    return new Query.PropertyQueryBuilder<T1>("Author", temp);
                }
            }
            
            /// Query for member 
            public virtual Query.PropertyQueryBuilder<T1> Content {
                get {
                    string temp = associationPath;
                    return new Query.PropertyQueryBuilder<T1>("Content", temp);
                }
            }
            
            /// Query for member 
            public virtual Query.QueryBuilder<T1> Id {
                get {
                    string temp = associationPath;
                    return new Query.QueryBuilder<T1>("Id", temp);
                }
            }
            
            /// Query for member 
            public virtual Query_Post<T1> Post {
                get {
                    string temp = associationPath;
                    temp = ((temp + ".") 
                                + "Post");
                    return new Query_Post<T1>("Post", temp, true);
                }
            }
        }
        
        /// Query for member Root_Query_Comment
        public partial class Root_Query_Comment : Query_Comment<NHibernate.Query.Generator.Tests.ActiveRecord.Comment> {
            
            /// Query for member .ctor
            public Root_Query_Comment() : 
                    base("this", null) {
            }
        }
    }
    
    public partial class OrderBy {
        
        /// Query for member Comment
        public partial class Comment {
            
            /// Query for member Author
            public static Query.OrderByClause Author {
                get {
                    return new Query.OrderByClause("Author");
                }
            }
            
            /// Query for member Content
            public static Query.OrderByClause Content {
                get {
                    return new Query.OrderByClause("Content");
                }
            }
            
            /// Query for member Id
            public static Query.OrderByClause Id {
                get {
                    return new Query.OrderByClause("Id");
                }
            }
        }
    }
    
    public partial class ProjectBy {
        
        /// Query for member Comment
        public partial class Comment {
            
            /// Query for member Author
            public static Query.PropertyProjectionBuilder Author {
                get {
                    return new Query.PropertyProjectionBuilder("Author");
                }
            }
            
            /// Query for member Content
            public static Query.PropertyProjectionBuilder Content {
                get {
                    return new Query.PropertyProjectionBuilder("Content");
                }
            }
        }
    }
    
    public partial class GroupBy {
        
        /// Query for member Comment
        public partial class Comment {
            
            /// Query for member Author
            public static NHibernate.Expression.IProjection Author {
                get {
                    return NHibernate.Expression.Projections.GroupProperty("Author");
                }
            }
            
            /// Query for member Content
            public static NHibernate.Expression.IProjection Content {
                get {
                    return NHibernate.Expression.Projections.GroupProperty("Content");
                }
            }
        }
    }
}