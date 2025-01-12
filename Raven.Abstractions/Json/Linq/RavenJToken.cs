using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Abstractions.Json;
using Raven.Imports.Newtonsoft.Json;
using Raven.Imports.Newtonsoft.Json.Linq;
using Raven.Json.Utilities;

namespace Raven.Json.Linq
{
    /// <summary>
    ///     Represents an abstract JSON token.
    /// </summary>
    public abstract class RavenJToken
    {
        private static readonly JsonSerializer defaultJsonSerializer = JsonExtensions.CreateDefaultJsonSerializer();
        /// <summary>
        ///     Gets the node type for this <see cref="RavenJToken" />.
        /// </summary>
        /// <value>The type.</value>
        public abstract JTokenType Type { get; }

        /// <summary>
        ///     Clones this object
        /// </summary>
        /// <returns>A cloned RavenJToken</returns>
        public abstract RavenJToken CloneToken();

        public abstract bool IsSnapshot { get; }

        public abstract void EnsureCannotBeChangeAndEnableSnapshotting();

        public abstract RavenJToken CreateSnapshot();

        protected RavenJToken CloneTokenImpl(RavenJToken newObject)
        {
            var readingStack = new Stack<IEnumerable<KeyValuePair<string, RavenJToken>>>();
            var writingStack = new Stack<RavenJToken>();

            writingStack.Push(newObject);
            readingStack.Push(GetCloningEnumerator());

            while (readingStack.Count > 0)
            {
                var curReader = readingStack.Pop();
                var curObject = writingStack.Pop();
                foreach (var current in curReader)
                {
                    if (current.Value == null)
                    {
                        curObject.AddForCloning(current.Key, null); // we call this explicitly to support null entries in JArray
                        continue;
                    }

                    if (current.Value is RavenJValue)
                    {
                        curObject.AddForCloning(current.Key, current.Value.CloneToken());
                        continue;
                    }

                    var newVal = current.Value is RavenJArray ? (RavenJToken) new RavenJArray() : new RavenJObject();

                    curObject.AddForCloning(current.Key, newVal);

                    writingStack.Push(newVal);
                    readingStack.Push(current.Value.GetCloningEnumerator());
                }
            }

            return newObject;
        }

        internal static RavenJToken FromObjectInternal(object o, JsonSerializer jsonSerializer)
        {
            var ravenJToken = o as RavenJToken;
            if (ravenJToken != null)
                return ravenJToken;

            RavenJToken token;
            using (var jsonWriter = new RavenJTokenWriter())
            {
                jsonSerializer.Serialize(jsonWriter, o);
                token = jsonWriter.Token;
            }

            return token;
        }

        /// <summary>
        /// Creates the specified .NET type from the <see cref="JToken"/> using the specified <see cref="JsonSerializer"/>.
        /// </summary>
        /// <typeparam name="T">The object type that the token will be deserialized to.</typeparam>
        /// <returns>The new object created from the JSON value.</returns>
        /// <remarks>Creates new instance of JsonSerializer for each call!</remarks>
        public T ToObject<T>()
            where T : class
        {
            return ToObject<T>(defaultJsonSerializer);
        }

        /// <summary>
        /// Creates the specified .NET type from the <see cref="JToken"/> using the specified <see cref="JsonSerializer"/>.
        /// </summary>
        /// <typeparam name="T">The object type that the token will be deserialized to.</typeparam>
        /// <param name="jsonSerializer">The <see cref="JsonSerializer"/> that will be used when creating the object.</param>
        /// <returns>The new object created from the JSON value.</returns>
        public T ToObject<T>(JsonSerializer jsonSerializer)
            where T : class
        {
            return jsonSerializer.Deserialize<T>(new RavenJTokenReader(this));
        }

        /// <summary>
        ///     Creates a <see cref="RavenJToken" /> from an object.
        /// </summary>
        /// <param name="o">The object that will be used to create <see cref="RavenJToken" />.</param>
        /// <returns>A <see cref="RavenJToken" /> with the value of the specified object</returns>
        public static RavenJToken FromObject(object o)
        {
            return FromObjectInternal(o, defaultJsonSerializer);
        }

        /// <summary>
        ///     Creates a <see cref="RavenJToken" /> from an object using the specified <see cref="JsonSerializer" />.
        /// </summary>
        /// <param name="o">The object that will be used to create <see cref="RavenJToken" />.</param>
        /// <param name="jsonSerializer">The <see cref="JsonSerializer" /> that will be used when reading the object.</param>
        /// <returns>A <see cref="RavenJToken" /> with the value of the specified object</returns>
        public static RavenJToken FromObject(object o, JsonSerializer jsonSerializer)
        {
            return FromObjectInternal(o, jsonSerializer);
        }

        /// <summary>
        ///     Returns the indented JSON for this token.
        /// </summary>
        /// <returns>
        ///     The indented JSON for this token.
        /// </returns>
        public override string ToString()
        {
            return ToString(Formatting.Indented);
        }

        /// <summary>
        ///     Returns the JSON for this token using the given formatting and converters.
        /// </summary>
        /// <param name="formatting">Indicates how the output is formatted.</param>
        /// <param name="converters">A collection of <see cref="JsonConverter" /> which will be used when writing the token.</param>
        /// <returns>The JSON for this token using the given formatting and converters.</returns>
        public string ToString(Formatting formatting, params JsonConverter[] converters)
        {
            using (var sw = new StringWriter(CultureInfo.InvariantCulture))
            {
                var jw = new JsonTextWriter(sw);
                jw.Formatting = formatting;

                WriteTo(jw, converters);

                return sw.ToString();
            }
        }

        /// <summary>
        ///     Writes this token to a <see cref="JsonWriter" />.
        /// </summary>
        /// <param name="writer">A <see cref="JsonWriter" /> into which this method will write.</param>
        /// <param name="converters">A collection of <see cref="JsonConverter" /> which will be used when writing the token.</param>
        public abstract void WriteTo(JsonWriter writer, JsonConverterCollection converters);

        /// <summary>
        ///     Writes this token to a <see cref="JsonWriter" />.
        /// </summary>
        /// <param name="writer">A <see cref="JsonWriter" /> into which this method will write.</param>
        /// <param name="converters">A collection of <see cref="JsonConverter" /> which will be used when writing the token.</param>
        public abstract void WriteTo(JsonWriter writer, params JsonConverter[] converters);

        /// <summary>
        ///     Creates a <see cref="RavenJToken" /> from a <see cref="JsonReader" />.
        /// </summary>
        /// <param name="reader">An <see cref="JsonReader" /> positioned at the token to read into this <see cref="RavenJToken" />.</param>
        /// <returns>
        ///     An <see cref="RavenJToken" /> that contains the token and its descendant tokens
        ///     that were read from the reader. The runtime type of the token is determined
        ///     by the token type of the first token encountered in the reader.
        /// </returns>
        public static RavenJToken ReadFrom(JsonReader reader)
        {
            if (reader.TokenType == JsonToken.None)
            {
                if (!reader.Read())
                    throw new Exception("Error reading RavenJToken from JsonReader.");
            }

            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    return RavenJObject.Load(reader);
                case JsonToken.StartArray:
                    return RavenJArray.Load(reader);
                case JsonToken.String:
                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.Date:
                case JsonToken.Boolean:
                case JsonToken.Bytes:
                case JsonToken.Null:
                case JsonToken.Undefined:
                    return new RavenJValue(reader.Value);
            }

            throw new Exception(StringUtils.FormatWith("Error reading RavenJToken from JsonReader. Unexpected token: {0}", CultureInfo.InvariantCulture, reader.TokenType));
        }

        /// <summary>
        ///     Load a <see cref="RavenJToken" /> from a string that contains JSON.
        /// </summary>
        /// <param name="json">A <see cref="string" /> that contains JSON.</param>
        /// <returns>A <see cref="RavenJToken" /> populated from the string that contains JSON.</returns>
        public static RavenJToken Parse(string json)
        {
            try
            {
                return ParseInternal(json);
            }
            catch (Exception e)
            {
                throw new JsonSerializationException("Could not parse: [" + json + "]", e);
            }
        }

        /// <summary>
        ///     Load a <see cref="RavenJToken" /> from a string that contains JSON.
        /// </summary>
        /// <param name="json">A <see cref="string" /> that contains JSON.</param>
        /// <param name="token">A <see cref="RavenJToken" /> populated from the string that contains JSON.</param>
        /// <returns>Returns true if parsing was succesful. False otherwise.</returns>
        public static bool TryParse(string json, out RavenJToken token)
        {
            try
            {
                token = ParseInternal(json);
                return true;
            }
            catch (Exception)
            {
                token = null;
                return false;
            }
        }

        private static RavenJToken ParseInternal(string json)
        {
            JsonReader jsonReader = new RavenJsonTextReader(new StringReader(json));
            return Load(jsonReader);
        }

        public static RavenJToken TryLoad(Stream stream)
        {
            var jsonTextReader = new RavenJsonTextReader(new StreamReader(stream));
            if (jsonTextReader.Read() == false || jsonTextReader.TokenType == JsonToken.None)
            {
                return null;
            }

            return Load(jsonTextReader);
        }

        public static async Task<RavenJToken> TryLoadAsync(Stream stream)
        {
            var jsonTextReader = new JsonTextReaderAsync(new StreamReader(stream));
            if (await jsonTextReader.ReadAsync().ConfigureAwait(false) == false || jsonTextReader.TokenType == JsonToken.None)
            {
                return null;
            }

            return await ReadFromAsync(jsonTextReader).ConfigureAwait(false);
        }

        /// <summary>
        ///     Creates a <see cref="RavenJToken" /> from a <see cref="JsonReader" />.
        /// </summary>
        /// <param name="reader">An <see cref="JsonReader" /> positioned at the token to read into this <see cref="RavenJToken" />.</param>
        /// <returns>
        ///     An <see cref="RavenJToken" /> that contains the token and its descendant tokens
        ///     that were read from the reader. The runtime type of the token is determined
        ///     by the token type of the first token encountered in the reader.
        /// </returns>
        public static RavenJToken Load(JsonReader reader)
        {
            return ReadFrom(reader);
        }

        /// <summary>
        ///     Gets the <see cref="RavenJToken" /> with the specified key converted to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to convert the token to.</typeparam>
        /// <param name="key">The token key.</param>
        /// <returns>The converted token value.</returns>
        public virtual T Value<T>(string key)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///     Compares the values of two tokens, including the values of all descendant tokens.
        /// </summary>
        /// <param name="t1">The first <see cref="RavenJToken" /> to compare.</param>
        /// <param name="t2">The second <see cref="RavenJToken" /> to compare.</param>
        /// <param name="difference"> changes description</param>
        /// <returns>true if the tokens are equal; otherwise false.</returns>
        public static bool DeepEquals(RavenJToken t1, RavenJToken t2, List<DocumentsChanges> difference)
        {
            return (t1 == t2 || (t1 != null && t2 != null && t1.DeepEquals(t2, difference)));
        }

        public static bool DeepEquals(RavenJToken t1, RavenJToken t2)
        {
            return (t1 == t2 || (t1 != null && t2 != null && t1.DeepEquals(t2)));
        }

        public static int GetDeepHashCode(RavenJToken t)
        {
            return t == null ? 0 : t.GetDeepHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal virtual bool DeepEquals(RavenJToken other)
        {
            return DeepEquals(other, (List<DocumentsChanges>) null);
        }


        internal virtual bool DeepEquals(RavenJToken other, List<DocumentsChanges> docChanges)
        {
            if (other == null)
                return false;

            if (CheckType(other.Type) == false)
                return false;

            var curType = JTokenType.None;
            var fieldName = string.Empty;
            var otherStack = new Stack<RavenJTokenState>();
            var thisStack = new Stack<RavenJToken>();
            var fieldNameStack = new Stack<string>();
            var isEqual = true;
            thisStack.Push(this);
            otherStack.Push(new RavenJTokenState(other, curType));

            while (otherStack.Count > 0)
            {
                var curOtherReader = otherStack.Peek();
                if (curOtherReader.WasHere)
                {
                    curType = curOtherReader.CurType;
                    otherStack.Pop();
                    continue;
                }

                curOtherReader.CurType = curType;
                curOtherReader.WasHere = true;
                var curThisReader = thisStack.Pop();

                string fieldArrName = string.Empty;

                if (fieldNameStack.Count > 0)
                {
                    fieldArrName = fieldNameStack.Pop();
                    fieldName = fieldArrName;
                }


                if (curOtherReader == null && curThisReader == null)
                    continue; // shouldn't happen, but we got an error report from a user about this
                if (curOtherReader == null || curThisReader == null)
                    return false;

                if (curThisReader.Type == curOtherReader.Token.Type)
                {
                    switch (curOtherReader.Token.Type)
                    {
                        case JTokenType.Array:
                            var selfArray = (RavenJArray) curThisReader;
                            var otherArray = (RavenJArray) curOtherReader.Token;
                            curType = JTokenType.Array;
                            if (selfArray.Length != otherArray.Length)
                            {
                                if (docChanges == null)
                                    return false;

                                isEqual = docChanges.CompareRavenJArrayData(selfArray, otherArray, fieldArrName);
                            }
                            else
                            {
                                for (var i = 0; i < selfArray.Length; i++)
                                {
                                    thisStack.Push(selfArray[i]);
                                    otherStack.Push(new RavenJTokenState(otherArray[i], curType, i));
                                    fieldNameStack.Push(fieldName);
                                }
                            }

                            break;
                        case JTokenType.Object:
                            var selfObj = (RavenJObject) curThisReader;
                            var otherObj = (RavenJObject) curOtherReader.Token;
                            if (selfObj.Count != otherObj.Count)
                            {
                                curType = JTokenType.Object;

                                if (docChanges == null)
                                    return false;
                                isEqual = docChanges.CompareDifferentLengthRavenJObjectData(otherObj, selfObj, fieldName);
                            }
                            else
                            {
                                var prevType = curType;
                                curType = JTokenType.Object;
                                var origFieldName = fieldName;
                                var hasChangedProperties = false;

                                foreach (var kvp in selfObj.Properties)
                                {
                                    fieldName = FieldName(prevType, origFieldName, kvp.Key, curOtherReader);

                                    RavenJToken token;
                                    if (otherObj.TryGetValue(kvp.Key, out token) == false)
                                    {
                                        if (docChanges == null)
                                            return false;

                                        if (token == null)
                                        {
                                            hasChangedProperties = true;
                                            continue;
                                        }

                                        docChanges.AddChanges(DocumentsChanges.ChangeType.RemovedField);
                                        isEqual = false;
                                    }

                                    if (kvp.Value == null)
                                    {
                                        if (token != null && token.Type != JTokenType.Null)
                                        {
                                            if (docChanges == null)
                                                return false;

                                            docChanges.AddChanges(DocumentsChanges.ChangeType.NewField);

                                            isEqual = false;
                                        }

                                        continue;
                                    }

                                    switch (kvp.Value.Type)
                                    {
                                        case JTokenType.Array:
                                        case JTokenType.Object:
                                            otherStack.Push(new RavenJTokenState(token, curType));
                                            thisStack.Push(kvp.Value);
                                            fieldNameStack.Push(fieldName);
                                            break;
                                        case JTokenType.Bytes:
                                            var bytes = kvp.Value.Value<byte[]>();
                                            var tokenBytes = token.Type == JTokenType.String
                                                ? Convert.FromBase64String(token.Value<string>())
                                                : token.Value<byte[]>();
                                            if (tokenBytes == null)
                                                return false;
                                            if (bytes.Length != tokenBytes.Length)
                                                return false;

                                            if (tokenBytes.Where((t, i) => t != bytes[i]).Any())
                                            {
                                                return false;
                                            }

                                            break;
                                        default:
                                            if (!kvp.Value.DeepEquals(token))
                                            {
                                                if (docChanges == null)
                                                    return false;
                                                docChanges.AddChanges(kvp, token, fieldName);
                                                isEqual = false;
                                            }

                                            break;
                                    }
                                }

                                if (hasChangedProperties)
                                {
                                    var addedProperties = selfObj.Properties.Keys.Except(otherObj.Properties.Keys).ToList();
                                    var removedProperties = otherObj.Properties.Keys.Except(selfObj.Properties.Keys).ToList();
                                    for (var i = 0; i < addedProperties.Count; i++)
                                    {
                                        RavenJToken newToken;
                                        if (selfObj.TryGetValue(addedProperties[i], out newToken) == false)
                                            return false;

                                        var newPropertyChanges = new DocumentsChanges
                                        {
                                            FieldNewValue = newToken.ToString(),
                                            FieldNewType = newToken.Type.ToString(),
                                            Change = DocumentsChanges.ChangeType.NewField,
                                            FieldName = addedProperties[i],
                                            FieldOldValue = "null",
                                            FieldOldType = "null"
                                        };

                                        RavenJToken oldToken;
                                        if (otherObj.TryGetValue(removedProperties[i], out oldToken) == false)
                                            return false;

                                        var oldPropertyChanges = new DocumentsChanges
                                        {
                                            FieldNewValue = "null",
                                            FieldNewType = "null",
                                            Change = DocumentsChanges.ChangeType.RemovedField,
                                            FieldName = removedProperties[i],
                                            FieldOldValue = oldToken.ToString(),
                                            FieldOldType = oldToken.Type.ToString()
                                        };

                                        docChanges.Add(newPropertyChanges);
                                        docChanges.Add(oldPropertyChanges);
                                        isEqual = false;
                                    }
                                }
                            }

                            break;
                        default:
                            curType = curThisReader.Type;
                            if (!curOtherReader.Token.DeepEquals(curThisReader))
                            {
                                if (docChanges == null)
                                    return false;

                                fieldName = FieldName(curOtherReader.CurType, fieldName, curThisReader.ToString(), curOtherReader, false);
                                docChanges.AddChanges(curThisReader, curOtherReader.Token, fieldName);
                                isEqual = false;
                            }

                            break;
                    }
                }
                else
                {
                    curType = curThisReader.Type;
                    switch (curThisReader.Type)
                    {
                        case JTokenType.Guid:
                            if (curOtherReader.Token.Type != JTokenType.String)
                                return false;

                            if (curThisReader.Value<string>() != curOtherReader.Token.Value<string>())
                                return false;

                            break;
                        default:
                            return false;
                    }
                }
            }

            return isEqual;
        }

        private static string FieldName(JTokenType prevType, string origFieldName, string key, RavenJTokenState curOtherReader, bool addKeyToFieldName = true)
        {
            string fieldName;

            if (prevType == JTokenType.Object)
            {
                fieldName = string.Format("{0}.{1}", origFieldName, key);
            }
            else if (prevType == JTokenType.Array)
            {
                fieldName = addKeyToFieldName ? string.Format("{0}[{1}].{2}", origFieldName, curOtherReader.Index, key) : string.Format("{0}[{1}]", origFieldName, curOtherReader.Index);
            }
            else
            {
                fieldName = key;
            }

            return fieldName;
        }

        private bool CheckType(JTokenType otherType)
        {
            switch (Type)
            {
                case JTokenType.Guid:
                case JTokenType.String:
                    if (otherType == JTokenType.String || otherType == JTokenType.Guid)
                        return true;
                    else
                        return false;
                default:
                    return Type == otherType;
            }
        }

        internal virtual int GetDeepHashCode()
        {
            var stack = new Stack<Tuple<int, RavenJToken>>();
            var ret = 0;

            stack.Push(Tuple.Create(0, this));
            while (stack.Count > 0)
            {
                var cur = stack.Pop();

                if (cur.Item2.Type == JTokenType.Array)
                {
                    var arr = (RavenJArray) cur.Item2;
                    for (var i = 0; i < arr.Length; i++)
                    {
                        stack.Push(Tuple.Create(cur.Item1 ^ (i * 397), arr[i]));
                    }
                }
                else if (cur.Item2.Type == JTokenType.Object)
                {
                    var selfObj = (RavenJObject) cur.Item2;
                    foreach (var kvp in selfObj.Properties)
                    {
                        stack.Push(Tuple.Create(cur.Item1 ^ (397 * kvp.Key.GetHashCode()), kvp.Value));
                    }
                }
                else // value
                {
                    ret ^= cur.Item1 ^ (cur.Item2.GetDeepHashCode() * 397);
                }
            }

            return ret;
        }


        /// <summary>
        ///     Selects the token that matches the object path.
        /// </summary>
        /// <param name="path">
        ///     The object path from the current <see cref="RavenJToken" /> to the <see cref="RavenJToken" />
        ///     to be returned. This must be a string of property names or array indexes separated
        ///     by periods, such as <code>Tables[0].DefaultView[0].Price</code> in C# or
        ///     <code>Tables(0).DefaultView(0).Price</code> in Visual Basic.
        /// </param>
        /// <returns>The <see cref="RavenJToken" /> that matches the object path or a null reference if no matching token is found.</returns>
        public RavenJToken SelectToken(string path)
        {
            return SelectToken(path, false);
        }

        /// <summary>
        ///     Selects the token that matches the object path.
        /// </summary>
        /// <param name="path">
        ///     The object path from the current <see cref="RavenJToken" /> to the <see cref="RavenJToken" />
        ///     to be returned. This must be a string of property names or array indexes separated
        ///     by periods, such as <code>Tables[0].DefaultView[0].Price</code> in C# or
        ///     <code>Tables(0).DefaultView(0).Price</code> in Visual Basic.
        /// </param>
        /// <param name="errorWhenNoMatch">A flag to indicate whether an error should be thrown if no token is found.</param>
        /// <param name="createSnapshots">A flag to indicate whether token snapshots should be created.</param>
        /// <returns>The <see cref="RavenJToken" /> that matches the object path.</returns>
        public RavenJToken SelectToken(string path, bool errorWhenNoMatch, bool createSnapshots = false)
        {
            var p = new RavenJPath(path);
            return p.Evaluate(this, errorWhenNoMatch, createSnapshots);
        }

        /// <summary>
        ///     Selects the token that matches the object path.
        /// </summary>
        /// <param name="path">
        ///     The object path from the current <see cref="RavenJToken" /> to the <see cref="RavenJToken" />
        ///     to be returned. This must be a string of property names or array indexes separated
        ///     by periods, such as <code>Tables[0].DefaultView[0].Price</code> in C# or
        ///     <code>Tables(0).DefaultView(0).Price</code> in Visual Basic.
        /// </param>
        /// <returns>The <see cref="RavenJToken" /> that matches the object path or a null reference if no matching token is found.</returns>
        public RavenJToken SelectToken(RavenJPath path)
        {
            return SelectToken(path, false);
        }

        /// <summary>
        ///     Selects the token that matches the object path.
        /// </summary>
        /// <param name="path">
        ///     The object path from the current <see cref="RavenJToken" /> to the <see cref="RavenJToken" />
        ///     to be returned. This must be a string of property names or array indexes separated
        ///     by periods, such as <code>Tables[0].DefaultView[0].Price</code> in C# or
        ///     <code>Tables(0).DefaultView(0).Price</code> in Visual Basic.
        /// </param>
        /// <param name="errorWhenNoMatch">A flag to indicate whether an error should be thrown if no token is found.</param>
        /// <param name="createSnapshots">A flag to indicate whether token snapshots should be created.</param>
        /// <returns>The <see cref="RavenJToken" /> that matches the object path.</returns>
        public RavenJToken SelectToken(RavenJPath path, bool errorWhenNoMatch, bool createSnapshots = false)
        {
            return path.Evaluate(this, errorWhenNoMatch, createSnapshots);
        }

        /// <summary>
        ///     Returns a collection of the child values of this token, in document order.
        /// </summary>
        /// <typeparam name="T">The type to convert the values to.</typeparam>
        /// <returns>
        ///     A <see cref="IEnumerable{T}" /> containing the child values of this <see cref="RavenJToken" />, in document order.
        /// </returns>
        public virtual IEnumerable<T> Values<T>()
        {
            throw new NotSupportedException();
        }

        public virtual T Value<T>()
        {
            return this.Convert<T>();
        }

        /// <summary>
        ///     Returns a collection of the child values of this token, in document order.
        /// </summary>
        public virtual IEnumerable<RavenJToken> Values()
        {
            throw new NotSupportedException();
        }

        internal virtual void AddForCloning(string key, RavenJToken token)
        {
            // kept virtual (as opposed to abstract) to waive the new for implementing this in RavenJValue
        }

        internal virtual IEnumerable<KeyValuePair<string, RavenJToken>> GetCloningEnumerator()
        {
            return null;
        }

        #region Cast to operators

        /// <summary>
        ///     Performs an implicit conversion from <see cref="bool" /> to <see cref="RavenJToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="RavenJValue" /> from.</param>
        /// <returns>The <see cref="RavenJValue" /> initialized with the specified value.</returns>
        public static implicit operator RavenJToken(bool value)
        {
            return new RavenJValue(value);
        }

#if !PocketPC && !NET20
        /// <summary>
        ///     Performs an implicit conversion from <see cref="DateTimeOffset" /> to <see cref="RavenJToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="RavenJValue" /> from.</param>
        /// <returns>The <see cref="RavenJValue" /> initialized with the specified value.</returns>
        public static implicit operator RavenJToken(DateTimeOffset value)
        {
            return new RavenJValue(value);
        }
#endif

        /// <summary>
        ///     Performs an implicit conversion from <see cref="Nullable{Boolean}" /> to <see cref="RavenJToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="RavenJValue" /> from.</param>
        /// <returns>The <see cref="RavenJValue" /> initialized with the specified value.</returns>
        public static implicit operator RavenJToken(bool? value)
        {
            return new RavenJValue(value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="Nullable{Int64}" /> to <see cref="RavenJToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="RavenJValue" /> from.</param>
        /// <returns>The <see cref="RavenJValue" /> initialized with the specified value.</returns>
        public static implicit operator RavenJToken(long value)
        {
            return new RavenJValue(value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="Nullable{DateTime}" /> to <see cref="RavenJToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="RavenJValue" /> from.</param>
        /// <returns>The <see cref="RavenJValue" /> initialized with the specified value.</returns>
        public static implicit operator RavenJToken(DateTime? value)
        {
            return new RavenJValue(value);
        }

#if !PocketPC && !NET20
        /// <summary>
        ///     Performs an implicit conversion from <see cref="Nullable{DateTimeOffset}" /> to <see cref="RavenJToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="RavenJValue" /> from.</param>
        /// <returns>The <see cref="RavenJValue" /> initialized with the specified value.</returns>
        public static implicit operator RavenJToken(DateTimeOffset? value)
        {
            return new RavenJValue(value);
        }
#endif

        /// <summary>
        ///     Performs an implicit conversion from <see cref="Nullable{Decimal}" /> to <see cref="RavenJToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="RavenJValue" /> from.</param>
        /// <returns>The <see cref="RavenJValue" /> initialized with the specified value.</returns>
        public static implicit operator RavenJToken(decimal? value)
        {
            return new RavenJValue(value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="Nullable{Double}" /> to <see cref="RavenJToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="RavenJValue" /> from.</param>
        /// <returns>The <see cref="RavenJValue" /> initialized with the specified value.</returns>
        public static implicit operator RavenJToken(double? value)
        {
            return new RavenJValue(value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="Int16" /> to <see cref="RavenJToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="RavenJValue" /> from.</param>
        /// <returns>The <see cref="RavenJValue" /> initialized with the specified value.</returns>
        [CLSCompliant(false)]
        public static implicit operator RavenJToken(short value)
        {
            return new RavenJValue(value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="UInt16" /> to <see cref="RavenJToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="RavenJValue" /> from.</param>
        /// <returns>The <see cref="RavenJValue" /> initialized with the specified value.</returns>
        [CLSCompliant(false)]
        public static implicit operator RavenJToken(ushort value)
        {
            return new RavenJValue(value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="Int32" /> to <see cref="RavenJToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="RavenJValue" /> from.</param>
        /// <returns>The <see cref="RavenJValue" /> initialized with the specified value.</returns>
        public static implicit operator RavenJToken(int value)
        {
            return new RavenJValue(value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="Nullable{Int32}" /> to <see cref="RavenJToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="RavenJValue" /> from.</param>
        /// <returns>The <see cref="RavenJValue" /> initialized with the specified value.</returns>
        public static implicit operator RavenJToken(int? value)
        {
            return new RavenJValue(value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="DateTime" /> to <see cref="RavenJToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="RavenJValue" /> from.</param>
        /// <returns>The <see cref="RavenJValue" /> initialized with the specified value.</returns>
        public static implicit operator RavenJToken(DateTime value)
        {
            return new RavenJValue(value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="Nullable{Int64}" /> to <see cref="RavenJToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="RavenJValue" /> from.</param>
        /// <returns>The <see cref="RavenJValue" /> initialized with the specified value.</returns>
        public static implicit operator RavenJToken(long? value)
        {
            return new RavenJValue(value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="Nullable{Single}" /> to <see cref="RavenJToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="RavenJValue" /> from.</param>
        /// <returns>The <see cref="RavenJValue" /> initialized with the specified value.</returns>
        public static implicit operator RavenJToken(float? value)
        {
            return new RavenJValue(value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="Decimal" /> to <see cref="RavenJToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="RavenJValue" /> from.</param>
        /// <returns>The <see cref="RavenJValue" /> initialized with the specified value.</returns>
        public static implicit operator RavenJToken(decimal value)
        {
            return new RavenJValue(value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="Nullable{Int16}" /> to <see cref="RavenJToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="RavenJValue" /> from.</param>
        /// <returns>The <see cref="RavenJValue" /> initialized with the specified value.</returns>
        [CLSCompliant(false)]
        public static implicit operator RavenJToken(short? value)
        {
            return new RavenJValue(value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="Nullable{UInt16}" /> to <see cref="RavenJToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="RavenJValue" /> from.</param>
        /// <returns>The <see cref="RavenJValue" /> initialized with the specified value.</returns>
        [CLSCompliant(false)]
        public static implicit operator RavenJToken(ushort? value)
        {
            return new RavenJValue(value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="Nullable{UInt32}" /> to <see cref="RavenJToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="RavenJValue" /> from.</param>
        /// <returns>The <see cref="RavenJValue" /> initialized with the specified value.</returns>
        [CLSCompliant(false)]
        public static implicit operator RavenJToken(uint? value)
        {
            return new RavenJValue(value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="Nullable{UInt64}" /> to <see cref="RavenJToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="RavenJValue" /> from.</param>
        /// <returns>The <see cref="RavenJValue" /> initialized with the specified value.</returns>
        [CLSCompliant(false)]
        public static implicit operator RavenJToken(ulong? value)
        {
            return new RavenJValue(value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="Double" /> to <see cref="RavenJToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="RavenJValue" /> from.</param>
        /// <returns>The <see cref="RavenJValue" /> initialized with the specified value.</returns>
        public static implicit operator RavenJToken(double value)
        {
            return new RavenJValue(value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="Single" /> to <see cref="RavenJToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="RavenJValue" /> from.</param>
        /// <returns>The <see cref="RavenJValue" /> initialized with the specified value.</returns>
        public static implicit operator RavenJToken(float value)
        {
            return new RavenJValue(value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="String" /> to <see cref="RavenJToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="RavenJValue" /> from.</param>
        /// <returns>The <see cref="RavenJValue" /> initialized with the specified value.</returns>
        public static implicit operator RavenJToken(string value)
        {
            return new RavenJValue(value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="UInt32" /> to <see cref="RavenJToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="RavenJValue" /> from.</param>
        /// <returns>The <see cref="RavenJValue" /> initialized with the specified value.</returns>
        [CLSCompliant(false)]
        public static implicit operator RavenJToken(uint value)
        {
            return new RavenJValue(value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="UInt64" /> to <see cref="RavenJToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="RavenJValue" /> from.</param>
        /// <returns>The <see cref="RavenJValue" /> initialized with the specified value.</returns>
        [CLSCompliant(false)]
        public static implicit operator RavenJToken(ulong value)
        {
            return new RavenJValue(value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="T:System.Byte[]" /> to <see cref="RavenJToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="RavenJValue" /> from.</param>
        /// <returns>The <see cref="RavenJValue" /> initialized with the specified value.</returns>
        public static implicit operator RavenJToken(byte[] value)
        {
            return new RavenJValue(value);
        }

        #endregion

        public static async Task<RavenJToken> ReadFromAsync(JsonTextReaderAsync reader)
        {
            if (reader.TokenType == JsonToken.None)
            {
                if (!await reader.ReadAsync().ConfigureAwait(false))
                    throw new Exception("Error reading RavenJToken from JsonReader.");
            }

            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    return await RavenJObject.LoadAsync(reader).ConfigureAwait(false);
                case JsonToken.StartArray:
                    return await RavenJArray.LoadAsync(reader).ConfigureAwait(false);
                case JsonToken.String:
                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.Date:
                case JsonToken.Boolean:
                case JsonToken.Bytes:
                case JsonToken.Null:
                case JsonToken.Undefined:
                    return new RavenJValue(reader.Value);
            }

            throw new Exception(StringUtils.FormatWith("Error reading RavenJToken from JsonReader. Unexpected token: {0}", CultureInfo.InvariantCulture, reader.TokenType));
        }
    }

    public class RavenJTokenState
    {
        public JTokenType CurType;
        public bool WasHere;
        public int Index { get; }

        public RavenJToken Token { get; }

        public RavenJTokenState(RavenJToken token, JTokenType curType, int index = -1, bool wasHere = false)
        {
            Token = token;
            CurType = curType;
            WasHere = wasHere;
            Index = index;
        }
    }
}