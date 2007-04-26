using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Castle.Components.Validator;
using Castle.Core;
using log4net;
using Rhino.Commons;

namespace Rhino.Igloo
{
	/// <summary>
	/// Base class for all the controllers
	/// </summary>
    [Transient]
	public class BaseController
	{
	    private static IValidatorRegistry validationRegistry = new CachedValidationRegistry();
        
        private IContext context;
    	private ILog log;
    	 private ValidatorRunner validatorRunner;


	    /// <summary>
		/// Initializes a new instance of the <see cref="BaseController"/> class.
		/// </summary>
		/// <param myName="context">The context.</param>
    	public BaseController(IContext context)
		{
		    this.context = context;
		}

       

		/// <summary>
		/// Gets or sets the log for this controller.
		/// </summary>
		/// <value>The log.</value>
    	public ILog Log
    	{
    		get { return log; }
    		set { log = value; }
    	}

		

		/// <summary>
		/// Gets the context we are currently in.
		/// </summary>
		/// <value>The context.</value>
    	public IContext Context
		{
			get
			{
				return context;
			}
		}

        
        /// <summary>
        /// Return an an empty list of <typeparamref name="T"/>.
        /// Used to clarify the intent of the action only
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The whole point here is to get EmptyList<Type> support")]
        public static IList<T> EmptyList<T>()
        {
            return new List<T>();
        }


        /// <summary>
        /// Tries to parse the inputName as int.
        /// </summary>
        /// <param name="inputName">The inputName.</param>
        /// <returns></returns>
        protected static bool? TryParseBoolFromInput(string inputName)
        {
            bool b;
            string userInput = Scope.Input[inputName];
            if (bool.TryParse(userInput, out b))
                return b;
            return null;
        }

        /// <summary>
        /// Tries to parse the inputName as int.
        /// </summary>
        /// <param name="inputName">The inputName.</param>
        /// <returns></returns>
        protected static bool ParseCheckBox(string inputName)
        {
            string userInput = Scope.Input[inputName];
            if (userInput == "on")
                return true;
            return false;
        }

        /// <summary>
        /// Tries to parse the inputName as int.
        /// </summary>
        /// <param name="inputName">The inputName.</param>
        /// <returns></returns>
        protected static int? TryParseInt32FromInput(string inputName)
        {
            int i;
            string userInput = Scope.Input[inputName];
            if (int.TryParse(userInput, out i))
                return i;
            return null;
        }

        /// <summary>
        /// Tries the parse userInput as integer.
        /// </summary>
        /// <param name="userInput">The user input.</param>
        /// <returns></returns>
        protected static int? TryParseInt32(string userInput)
        {
            int i;
            if (int.TryParse(userInput, out i))
                return i;
            return null;
        }


        /// <summary>
        /// Tries to parse the date using dd/MM/yyyy format
        /// </summary>
        /// <param name="inputName">Name of the input.</param>
        /// <returns></returns>
        protected static DateTime? TryParseDateFromInput(string inputName)
        {
            return TryParseDateFromInput(inputName, "dd/MM/yyyy");
        }

        /// <summary>
        /// Tries to parse the inputName as date.
        /// </summary>
        /// <param name="inputName">The inputName.</param>
        /// <param name="formats">The formats.</param>
        /// <returns></returns>
        protected static DateTime? TryParseDateFromInput(string inputName, params string[] formats)
        {
            DateTime datetime;
            string userInput = Scope.Input[inputName];
            if (DateTime.TryParseExact(userInput, formats,CultureInfo.InvariantCulture,DateTimeStyles.None,out datetime))
                return datetime;
            return null;
        }

        /// <summary>
        /// Tries to get the value for the input key.
        /// </summary>
        /// <param name="inputKey">The input key.</param>
        /// <returns></returns>
        protected static T TryGetFromInput<T>(string inputKey)
            where T : class
        {
            string maybeId = Scope.Input[inputKey];
            return TryGetById<T>(maybeId);
        }

        /// <summary>
        /// Tries to get the value for the input key.
        /// </summary>
        /// <param name="inputKey">The input key.</param>
        /// <returns></returns>
        protected static T TryGetFromInputString<T>(string inputKey)
            where T : class
        {
            string maybeId = Scope.Input[inputKey];
            return TryGetByIdString<T>(maybeId);
        }

        /// <summary>
        /// Tries the get by id.
        /// </summary>
        /// <param name="maybeId">The maybe id.</param>
        /// <returns></returns>
	    protected static T TryGetById<T>(string maybeId)
                where T : class
	    {
	        int id;
	        if (int.TryParse(maybeId, out id) == false)
	            return null;
	        return Repository<T>.Get(id);
	    }

        /// <summary>
        /// Tries the get by id.
        /// </summary>
        /// <param name="maybeId">The maybe id.</param>
        /// <returns></returns>
        protected static T TryGetByIdString<T>(string maybeId)
                where T : class
        {
            if (maybeId == null)
                return null;
            return Repository<T>.Get(maybeId);
        }

        /// <summary>
        /// Gets the validator runner.
        /// </summary>
        /// <value>The validator runner.</value>
	    protected ValidatorRunner ValidatorRunner
	    {
	        get
	        {
                if(validatorRunner==null)
                    validatorRunner = new ValidatorRunner(validationRegistry);
	            return validatorRunner;
	        }
	    }
	}
}