//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.312
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Query {
    
    
    public partial class Where {
        
        /// <summary>
        /// Query helper for member Where.Customer
        /// </summary>
        public static Root_Query_Customer Customer {
            get {
                return new Root_Query_Customer();
            }
        }
        
        /// <summary>
        /// Query helper for member Where.Address
        /// </summary>
        public static Root_Query_Address Address {
            get {
                return new Root_Query_Address();
            }
        }
        
        /// <summary>
        /// Query helper for member Where.CompositeCustomer
        /// </summary>
        public static Root_Query_CompositeCustomer CompositeCustomer {
            get {
                return new Root_Query_CompositeCustomer();
            }
        }
        
        /// <summary>
        /// Query helper for member Where.BadCustomer
        /// </summary>
        public static Root_Query_BadCustomer BadCustomer {
            get {
                return new Root_Query_BadCustomer();
            }
        }
        
        /// <summary>
        /// Query helper for member Where.BadCustomer2
        /// </summary>
        public static Root_Query_BadCustomer2 BadCustomer2 {
            get {
                return new Root_Query_BadCustomer2();
            }
        }
        
        /// <summary>
        /// Query helper for member Where.ValuedCustomer
        /// </summary>
        public static Root_Query_ValuedCustomer ValuedCustomer {
            get {
                return new Root_Query_ValuedCustomer();
            }
        }
        
        /// <summary>
        /// Query helper for member Where.ValuedCustomer2
        /// </summary>
        public static Root_Query_ValuedCustomer2 ValuedCustomer2 {
            get {
                return new Root_Query_ValuedCustomer2();
            }
        }
        
        /// <summary>
        /// Query helper for member Where.Query_Customer
        /// </summary>
        public partial class Query_Customer<T1> : QueryBuilder<T1>
         {
            
            /// <summary>
            /// Query helper for member Query_Customer..ctor
            /// </summary>
            public Query_Customer(QueryBuilder<T1> parent, string name, string associationPath) : 
                    base(parent, name, associationPath) {
            }
            
            /// <summary>
            /// Query helper for member Query_Customer..ctor
            /// </summary>
            public Query_Customer(QueryBuilder<T1> parent, string name, string associationPath, bool backTrackAssociationOnEquality) : 
                    base(parent, name, associationPath, backTrackAssociationOnEquality) {
            }
            
            /// <summary>
            /// Query helper for member Query_Customer.
            /// </summary>
            public virtual PropertyQueryBuilder<T1> Name {
                get {
                    string temp = associationPath;
                    return new PropertyQueryBuilder<T1>(this, "Name", temp);
                }
            }
            
            /// <summary>
            /// Query helper for member Query_Customer.
            /// </summary>
            public virtual QueryBuilder<T1> Id {
                get {
                    string temp = associationPath;
                    return new QueryBuilder<T1>(this, "Id", temp);
                }
            }
            
            /// <summary>
            /// Query helper for member Query_Customer.
            /// </summary>
            public virtual Query_Address<T1> Address {
                get {
                    string temp = associationPath;
                    temp = ((temp + ".") 
                                + "Address");
                    return new Query_Address<T1>(this, "Address", temp, true);
                }
            }
            
            /// <summary>
            /// Query helper for member Query_Customer.Home
            /// </summary>
            public virtual Query_Home<T1> Home {
                get {
                    return new Query_Home<T1>(this, "Home", null);
                }
            }
            
            /// <summary>
            /// Query helper for member Query_Customer.Query_Home
            /// </summary>
            public partial class Query_Home<T2> : QueryBuilder<T2>
             {
                
                /// <summary>
                /// Query helper for member Query_Home..ctor
                /// </summary>
                public Query_Home(QueryBuilder<T2> parent, string name, string associationPath) : 
                        base(parent, name, associationPath) {
                }
                
                /// <summary>
                /// Query helper for member Query_Home..ctor
                /// </summary>
                public Query_Home(QueryBuilder<T2> parent, string name, string associationPath, bool backTrackAssociationOnEquality) : 
                        base(parent, name, associationPath, backTrackAssociationOnEquality) {
                }
                
                /// <summary>
                /// Query helper for member Query_Home.
                /// </summary>
                public virtual PropertyQueryBuilder<T2> Phone {
                    get {
                        string temp = associationPath;
                        return new PropertyQueryBuilder<T2>(this, "Home.Phone", temp);
                    }
                }
                
                /// <summary>
                /// Query helper for member Query_Home.
                /// </summary>
                public virtual Query_Address<T2> Address {
                    get {
                        string temp = associationPath;
                        temp = ((temp + ".") 
                                    + "Address");
                        return new Query_Address<T2>(this, "Home.Address", temp, true);
                    }
                }
            }
        }
        
        /// <summary>
        /// Query helper for member Where.Root_Query_Customer
        /// </summary>
        public partial class Root_Query_Customer : Query_Customer<My.Simple.Model.Customer> {
            
            /// <summary>
            /// Query helper for member Root_Query_Customer..ctor
            /// </summary>
            public Root_Query_Customer() : 
                    base(null, "this", null) {
            }
        }
        
        /// <summary>
        /// Query helper for member Where.Query_Address
        /// </summary>
        public partial class Query_Address<T3> : QueryBuilder<T3>
         {
            
            /// <summary>
            /// Query helper for member Query_Address..ctor
            /// </summary>
            public Query_Address(QueryBuilder<T3> parent, string name, string associationPath) : 
                    base(parent, name, associationPath) {
            }
            
            /// <summary>
            /// Query helper for member Query_Address..ctor
            /// </summary>
            public Query_Address(QueryBuilder<T3> parent, string name, string associationPath, bool backTrackAssociationOnEquality) : 
                    base(parent, name, associationPath, backTrackAssociationOnEquality) {
            }
            
            /// <summary>
            /// Query helper for member Query_Address.
            /// </summary>
            public virtual QueryBuilder<T3> Pk {
                get {
                    string temp = associationPath;
                    return new QueryBuilder<T3>(this, "Pk", temp);
                }
            }
        }
        
        /// <summary>
        /// Query helper for member Where.Root_Query_Address
        /// </summary>
        public partial class Root_Query_Address : Query_Address<My.Simple.Model.Address> {
            
            /// <summary>
            /// Query helper for member Root_Query_Address..ctor
            /// </summary>
            public Root_Query_Address() : 
                    base(null, "this", null) {
            }
        }
        
        /// <summary>
        /// Query helper for member Where.Query_CompositeCustomer
        /// </summary>
        public partial class Query_CompositeCustomer<T4> : QueryBuilder<T4>
         {
            
            /// <summary>
            /// Query helper for member Query_CompositeCustomer..ctor
            /// </summary>
            public Query_CompositeCustomer(QueryBuilder<T4> parent, string name, string associationPath) : 
                    base(parent, name, associationPath) {
            }
            
            /// <summary>
            /// Query helper for member Query_CompositeCustomer..ctor
            /// </summary>
            public Query_CompositeCustomer(QueryBuilder<T4> parent, string name, string associationPath, bool backTrackAssociationOnEquality) : 
                    base(parent, name, associationPath, backTrackAssociationOnEquality) {
            }
            
            /// <summary>
            /// Query helper for member Query_CompositeCustomer.
            /// </summary>
            public virtual PropertyQueryBuilder<T4> Name {
                get {
                    string temp = associationPath;
                    return new PropertyQueryBuilder<T4>(this, "Name", temp);
                }
            }
            
            /// <summary>
            /// Query helper for member Query_CompositeCustomer.
            /// </summary>
            public virtual PropertyQueryBuilder<T4> CustomerId {
                get {
                    string temp = associationPath;
                    return new PropertyQueryBuilder<T4>(this, "CustomerId", temp);
                }
            }
            
            /// <summary>
            /// Query helper for member Query_CompositeCustomer.
            /// </summary>
            public virtual Query_BadCustomer<T4> Foo {
                get {
                    string temp = associationPath;
                    temp = ((temp + ".") 
                                + "Foo");
                    return new Query_BadCustomer<T4>(this, "Foo", temp, true);
                }
            }
        }
        
        /// <summary>
        /// Query helper for member Where.Root_Query_CompositeCustomer
        /// </summary>
        public partial class Root_Query_CompositeCustomer : Query_CompositeCustomer<My.Simple.Model.CompositeCustomer> {
            
            /// <summary>
            /// Query helper for member Root_Query_CompositeCustomer..ctor
            /// </summary>
            public Root_Query_CompositeCustomer() : 
                    base(null, "this", null) {
            }
        }
        
        /// <summary>
        /// Query helper for member Where.Query_BadCustomer
        /// </summary>
        public partial class Query_BadCustomer<T5> : Query_Customer<T5>
         {
            
            /// <summary>
            /// Query helper for member Query_BadCustomer..ctor
            /// </summary>
            public Query_BadCustomer(QueryBuilder<T5> parent, string name, string associationPath) : 
                    base(parent, name, associationPath) {
            }
            
            /// <summary>
            /// Query helper for member Query_BadCustomer..ctor
            /// </summary>
            public Query_BadCustomer(QueryBuilder<T5> parent, string name, string associationPath, bool backTrackAssociationOnEquality) : 
                    base(parent, name, associationPath, backTrackAssociationOnEquality) {
            }
            
            /// <summary>
            /// Query helper for member Query_BadCustomer.
            /// </summary>
            public virtual PropertyQueryBuilder<T5> Foo {
                get {
                    string temp = associationPath;
                    return new PropertyQueryBuilder<T5>(this, "Foo", temp);
                }
            }
        }
        
        /// <summary>
        /// Query helper for member Where.Root_Query_BadCustomer
        /// </summary>
        public partial class Root_Query_BadCustomer : Query_BadCustomer<My.Simple.Model.BadCustomer> {
            
            /// <summary>
            /// Query helper for member Root_Query_BadCustomer..ctor
            /// </summary>
            public Root_Query_BadCustomer() : 
                    base(null, "this", null) {
            }
        }
        
        /// <summary>
        /// Query helper for member Where.Query_BadCustomer2
        /// </summary>
        public partial class Query_BadCustomer2<T6> : QueryBuilder<T6>
         {
            
            /// <summary>
            /// Query helper for member Query_BadCustomer2..ctor
            /// </summary>
            public Query_BadCustomer2(QueryBuilder<T6> parent, string name, string associationPath) : 
                    base(parent, name, associationPath) {
            }
            
            /// <summary>
            /// Query helper for member Query_BadCustomer2..ctor
            /// </summary>
            public Query_BadCustomer2(QueryBuilder<T6> parent, string name, string associationPath, bool backTrackAssociationOnEquality) : 
                    base(parent, name, associationPath, backTrackAssociationOnEquality) {
            }
            
            /// <summary>
            /// Query helper for member Query_BadCustomer2.
            /// </summary>
            public virtual PropertyQueryBuilder<T6> Foo {
                get {
                    string temp = associationPath;
                    return new PropertyQueryBuilder<T6>(this, "Foo", temp);
                }
            }
        }
        
        /// <summary>
        /// Query helper for member Where.Root_Query_BadCustomer2
        /// </summary>
        public partial class Root_Query_BadCustomer2 : Query_BadCustomer2<My.Simple.Model.BadCustomer2> {
            
            /// <summary>
            /// Query helper for member Root_Query_BadCustomer2..ctor
            /// </summary>
            public Root_Query_BadCustomer2() : 
                    base(null, "this", null) {
            }
        }
        
        /// <summary>
        /// Query helper for member Where.Query_ValuedCustomer
        /// </summary>
        public partial class Query_ValuedCustomer<T7> : Query_Customer<T7>
         {
            
            /// <summary>
            /// Query helper for member Query_ValuedCustomer..ctor
            /// </summary>
            public Query_ValuedCustomer(QueryBuilder<T7> parent, string name, string associationPath) : 
                    base(parent, name, associationPath) {
            }
            
            /// <summary>
            /// Query helper for member Query_ValuedCustomer..ctor
            /// </summary>
            public Query_ValuedCustomer(QueryBuilder<T7> parent, string name, string associationPath, bool backTrackAssociationOnEquality) : 
                    base(parent, name, associationPath, backTrackAssociationOnEquality) {
            }
            
            /// <summary>
            /// Query helper for member Query_ValuedCustomer.
            /// </summary>
            public virtual PropertyQueryBuilder<T7> Bar {
                get {
                    string temp = associationPath;
                    return new PropertyQueryBuilder<T7>(this, "Bar", temp);
                }
            }
        }
        
        /// <summary>
        /// Query helper for member Where.Root_Query_ValuedCustomer
        /// </summary>
        public partial class Root_Query_ValuedCustomer : Query_ValuedCustomer<My.Simple.Model.ValuedCustomer> {
            
            /// <summary>
            /// Query helper for member Root_Query_ValuedCustomer..ctor
            /// </summary>
            public Root_Query_ValuedCustomer() : 
                    base(null, "this", null) {
            }
        }
        
        /// <summary>
        /// Query helper for member Where.Query_ValuedCustomer2
        /// </summary>
        public partial class Query_ValuedCustomer2<T8> : QueryBuilder<T8>
         {
            
            /// <summary>
            /// Query helper for member Query_ValuedCustomer2..ctor
            /// </summary>
            public Query_ValuedCustomer2(QueryBuilder<T8> parent, string name, string associationPath) : 
                    base(parent, name, associationPath) {
            }
            
            /// <summary>
            /// Query helper for member Query_ValuedCustomer2..ctor
            /// </summary>
            public Query_ValuedCustomer2(QueryBuilder<T8> parent, string name, string associationPath, bool backTrackAssociationOnEquality) : 
                    base(parent, name, associationPath, backTrackAssociationOnEquality) {
            }
            
            /// <summary>
            /// Query helper for member Query_ValuedCustomer2.
            /// </summary>
            public virtual PropertyQueryBuilder<T8> Bar {
                get {
                    string temp = associationPath;
                    return new PropertyQueryBuilder<T8>(this, "Bar", temp);
                }
            }
        }
        
        /// <summary>
        /// Query helper for member Where.Root_Query_ValuedCustomer2
        /// </summary>
        public partial class Root_Query_ValuedCustomer2 : Query_ValuedCustomer2<My.Simple.Model.ValuedCustomer2> {
            
            /// <summary>
            /// Query helper for member Root_Query_ValuedCustomer2..ctor
            /// </summary>
            public Root_Query_ValuedCustomer2() : 
                    base(null, "this", null) {
            }
        }
    }
    
    public partial class OrderBy {
        
        /// <summary>
        /// Query helper for member OrderBy.Customer
        /// </summary>
        public partial class Customer {
            
            /// <summary>
            /// Query helper for member Customer.Name
            /// </summary>
            public static OrderByClause Name {
                get {
                    return new OrderByClause("Name");
                }
            }
            
            /// <summary>
            /// Query helper for member Customer.Id
            /// </summary>
            public static OrderByClause Id {
                get {
                    return new OrderByClause("Id");
                }
            }
            
            /// <summary>
            /// Query helper for member Customer.Home
            /// </summary>
            public partial class Home {
                
                /// <summary>
                /// Query helper for member Home.Phone
                /// </summary>
                public static OrderByClause Phone {
                    get {
                        return new OrderByClause("Home.Phone");
                    }
                }
            }
        }
        
        /// <summary>
        /// Query helper for member OrderBy.Address
        /// </summary>
        public partial class Address {
            
            /// <summary>
            /// Query helper for member Address.Pk
            /// </summary>
            public static OrderByClause Pk {
                get {
                    return new OrderByClause("Pk");
                }
            }
        }
        
        /// <summary>
        /// Query helper for member OrderBy.CompositeCustomer
        /// </summary>
        public partial class CompositeCustomer {
            
            /// <summary>
            /// Query helper for member CompositeCustomer.Name
            /// </summary>
            public static OrderByClause Name {
                get {
                    return new OrderByClause("Name");
                }
            }
            
            /// <summary>
            /// Query helper for member dummy.CustomerId
            /// </summary>
            /// <summary>
            /// Query helper for member CompositeCustomer.CustomerId
            /// </summary>
            public static OrderByClause CustomerId {
                get {
                    return new OrderByClause("dummy.CustomerId");
                }
            }
        }
        
        /// <summary>
        /// Query helper for member OrderBy.BadCustomer
        /// </summary>
        public partial class BadCustomer : Customer {
            
            /// <summary>
            /// Query helper for member BadCustomer.Foo
            /// </summary>
            public static OrderByClause Foo {
                get {
                    return new OrderByClause("Foo");
                }
            }
        }
        
        /// <summary>
        /// Query helper for member OrderBy.BadCustomer2
        /// </summary>
        public partial class BadCustomer2 {
            
            /// <summary>
            /// Query helper for member BadCustomer2.Foo
            /// </summary>
            public static OrderByClause Foo {
                get {
                    return new OrderByClause("Foo");
                }
            }
        }
        
        /// <summary>
        /// Query helper for member OrderBy.ValuedCustomer
        /// </summary>
        public partial class ValuedCustomer : Customer {
            
            /// <summary>
            /// Query helper for member ValuedCustomer.Bar
            /// </summary>
            public static OrderByClause Bar {
                get {
                    return new OrderByClause("Bar");
                }
            }
        }
        
        /// <summary>
        /// Query helper for member OrderBy.ValuedCustomer2
        /// </summary>
        public partial class ValuedCustomer2 {
            
            /// <summary>
            /// Query helper for member ValuedCustomer2.Bar
            /// </summary>
            public static OrderByClause Bar {
                get {
                    return new OrderByClause("Bar");
                }
            }
        }
    }
    
    public partial class Queries {
        
        /// <summary>
        /// Query helper for member Queries.MyQuery
        /// </summary>
        public static string MyQuery {
            get {
                return "MyQuery";
            }
        }
    }
    
    public partial class Queries {
        
        /// <summary>
        /// Query helper for member Queries.HerQuery
        /// </summary>
        public static string HerQuery {
            get {
                return "HerQuery";
            }
        }
    }
    
    public partial class ProjectBy {
        
        /// <summary>
        /// Query helper for member ProjectBy.Customer
        /// </summary>
        public partial class Customer {
            
            /// <summary>
            /// Query helper for member Customer.Name
            /// </summary>
            public static PropertyProjectionBuilder Name {
                get {
                    return new PropertyProjectionBuilder("Name");
                }
            }
            
            /// <summary>
            /// Query helper for member Customer.Id
            /// </summary>
            public static NumericPropertyProjectionBuilder Id {
                get {
                    return new NumericPropertyProjectionBuilder("Id");
                }
            }
            
            /// <summary>
            /// Query helper for member Customer.Home
            /// </summary>
            public partial class Home {
                
                /// <summary>
                /// Query helper for member Home.Phone
                /// </summary>
                public static PropertyProjectionBuilder Phone {
                    get {
                        return new PropertyProjectionBuilder("Home.Phone");
                    }
                }
            }
        }
        
        /// <summary>
        /// Query helper for member ProjectBy.Address
        /// </summary>
        public partial class Address {
            
            /// <summary>
            /// Query helper for member Address.Pk
            /// </summary>
            public static PropertyProjectionBuilder Pk {
                get {
                    return new PropertyProjectionBuilder("Pk");
                }
            }
        }
        
        /// <summary>
        /// Query helper for member ProjectBy.CompositeCustomer
        /// </summary>
        public partial class CompositeCustomer {
            
            /// <summary>
            /// Query helper for member CompositeCustomer.Name
            /// </summary>
            public static PropertyProjectionBuilder Name {
                get {
                    return new PropertyProjectionBuilder("Name");
                }
            }
            
            /// <summary>
            /// Query helper for member dummy.CustomerId
            /// </summary>
            /// <summary>
            /// Query helper for member CompositeCustomer.CustomerId
            /// </summary>
            public static NumericPropertyProjectionBuilder CustomerId {
                get {
                    return new NumericPropertyProjectionBuilder("dummy.CustomerId");
                }
            }
        }
        
        /// <summary>
        /// Query helper for member ProjectBy.BadCustomer
        /// </summary>
        public partial class BadCustomer : Customer {
            
            /// <summary>
            /// Query helper for member BadCustomer.Foo
            /// </summary>
            public static PropertyProjectionBuilder Foo {
                get {
                    return new PropertyProjectionBuilder("Foo");
                }
            }
        }
        
        /// <summary>
        /// Query helper for member ProjectBy.BadCustomer2
        /// </summary>
        public partial class BadCustomer2 {
            
            /// <summary>
            /// Query helper for member BadCustomer2.Foo
            /// </summary>
            public static PropertyProjectionBuilder Foo {
                get {
                    return new PropertyProjectionBuilder("Foo");
                }
            }
        }
        
        /// <summary>
        /// Query helper for member ProjectBy.ValuedCustomer
        /// </summary>
        public partial class ValuedCustomer : Customer {
            
            /// <summary>
            /// Query helper for member ValuedCustomer.Bar
            /// </summary>
            public static PropertyProjectionBuilder Bar {
                get {
                    return new PropertyProjectionBuilder("Bar");
                }
            }
        }
        
        /// <summary>
        /// Query helper for member ProjectBy.ValuedCustomer2
        /// </summary>
        public partial class ValuedCustomer2 {
            
            /// <summary>
            /// Query helper for member ValuedCustomer2.Bar
            /// </summary>
            public static PropertyProjectionBuilder Bar {
                get {
                    return new PropertyProjectionBuilder("Bar");
                }
            }
        }
    }
    
    public partial class GroupBy {
        
        /// <summary>
        /// Query helper for member GroupBy.Customer
        /// </summary>
        public partial class Customer {
            
            /// <summary>
            /// Query helper for member Customer.Name
            /// </summary>
            public static NHibernate.Expression.IProjection Name {
                get {
                    return NHibernate.Expression.Projections.GroupProperty("Name");
                }
            }
            
            /// <summary>
            /// Query helper for member Customer.Home
            /// </summary>
            public partial class Home {
                
                /// <summary>
                /// Query helper for member Home.Phone
                /// </summary>
                public static NHibernate.Expression.IProjection Phone {
                    get {
                        return NHibernate.Expression.Projections.GroupProperty("Home.Phone");
                    }
                }
            }
        }
        
        /// <summary>
        /// Query helper for member GroupBy.CompositeCustomer
        /// </summary>
        public partial class CompositeCustomer {
            
            /// <summary>
            /// Query helper for member CompositeCustomer.Name
            /// </summary>
            public static NHibernate.Expression.IProjection Name {
                get {
                    return NHibernate.Expression.Projections.GroupProperty("Name");
                }
            }
            
            /// <summary>
            /// Query helper for member dummy.CustomerId
            /// </summary>
            /// <summary>
            /// Query helper for member CompositeCustomer.CustomerId
            /// </summary>
            public static NHibernate.Expression.IProjection CustomerId {
                get {
                    return NHibernate.Expression.Projections.GroupProperty("dummy.CustomerId");
                }
            }
        }
        
        /// <summary>
        /// Query helper for member GroupBy.BadCustomer
        /// </summary>
        public partial class BadCustomer : Customer {
            
            /// <summary>
            /// Query helper for member BadCustomer.Foo
            /// </summary>
            public static NHibernate.Expression.IProjection Foo {
                get {
                    return NHibernate.Expression.Projections.GroupProperty("Foo");
                }
            }
        }
        
        /// <summary>
        /// Query helper for member GroupBy.BadCustomer2
        /// </summary>
        public partial class BadCustomer2 {
            
            /// <summary>
            /// Query helper for member BadCustomer2.Foo
            /// </summary>
            public static NHibernate.Expression.IProjection Foo {
                get {
                    return NHibernate.Expression.Projections.GroupProperty("Foo");
                }
            }
        }
        
        /// <summary>
        /// Query helper for member GroupBy.ValuedCustomer
        /// </summary>
        public partial class ValuedCustomer : Customer {
            
            /// <summary>
            /// Query helper for member ValuedCustomer.Bar
            /// </summary>
            public static NHibernate.Expression.IProjection Bar {
                get {
                    return NHibernate.Expression.Projections.GroupProperty("Bar");
                }
            }
        }
        
        /// <summary>
        /// Query helper for member GroupBy.ValuedCustomer2
        /// </summary>
        public partial class ValuedCustomer2 {
            
            /// <summary>
            /// Query helper for member ValuedCustomer2.Bar
            /// </summary>
            public static NHibernate.Expression.IProjection Bar {
                get {
                    return NHibernate.Expression.Projections.GroupProperty("Bar");
                }
            }
        }
    }
}
