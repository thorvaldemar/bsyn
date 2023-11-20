using System.Text.RegularExpressions;

namespace BSYN {
    public class Bsyn {

        string jsonObject = "";

        public string stringified => "{" + jsonObject + "}";

        public void append(string key, string value) {
            if (jsonObject.Length > 0) jsonObject += ", ";
            jsonObject += $"\"{key}\": \"{value}\"";
        }

        public void append(string key, int value) { appendNum(key, value); }
        public void append(string key, long value) { appendNum(key, value); }
        public void append(string key, double value) { appendNum(key, value); }
        public void append(string key, float value) { appendNum(key, value); }
        public void append(string key, bool value) { appendNum(key, value.ToString().ToLower()); }

        public void append(string key, string[] value) { appendArray(key, PackArray(value)); }
        public void append(string key, int[] value) { appendArray(key, PackArray(value)); }
        public void append(string key, Bsyn[] value) { appendNum(key, PackArray(value)); }

        public void append(string key, Bsyn value) { appendNum(key, value.stringified); }


        void appendArray(string key, string value) {
            if (jsonObject.Length > 0) jsonObject += ", ";
            jsonObject += $"\"{key}\": {value}";
        }

        void appendNum(string key, object value) {
            if (jsonObject.Length > 0) jsonObject += ", ";
            jsonObject += $"\"{key}\": {value}";
        }

        public static string PackArray(string[] arr) {
            string json = "[";
            for (int i = 0; i < arr.Length; i++) {
                json += (i > 0 ? "," : "") + $"\"{arr[i]}\"";
            }
            return json + "]";
        }

        public static string PackArray(int[] arr) {
            string json = "[";
            for (int i = 0; i < arr.Length; i++) {
                json += (i > 0 ? "," : "") + $"{arr[i]}";
            }
            return json + "]";
        }

        public static string PackArray(Bsyn[] arr) {
            string json = "[";
            for (int i = 0; i < arr.Length; i++) {
                json += (i > 0 ? "," : "") + $"{arr[i].stringified}";
            }
            return json + "]";
        }

        public void clear() { jsonObject = ""; }


    }

    public class BsynObject {
        List<BsynObject> bsynObjects = new List<BsynObject>();
        BsynValueType type = BsynValueType.Object;
        string name = "";
        string value = "";

        public BsynObject(string str, string name = "BSYN object", BsynValueType valtype = BsynValueType.Object) {
            this.name = name;
            value = str;

            if (valtype == BsynValueType.Object || valtype == BsynValueType.Array) {
                string currentVarTitle = "",
                       currentVarContent = "";
                int type = -1,
                    bracks = 0;
                bool inVar = false,
                     inQuotes = false,
                     hasStarted = false,
                     typeWait = false,
                     inTitle = false;
                foreach (char c in str) {
                    if (!hasStarted) {
                        if (c == '{') {
                            hasStarted = true;
                            valtype = BsynValueType.Object;
                            //bracks++;
                        } else if (c == '[') {
                            hasStarted = true;
                            valtype = BsynValueType.Array;
                        } else {
                            continue;
                        }
                    }

                    if (!inQuotes) {
                        if (c == '{' || c == '[') bracks++;
                        if (c == '}' || c == ']') bracks--;
                    }

                    if (bracks < 0) throw new Exception("Closing to many brackets without opening them first");

                    if (!inVar) {
                        if (!typeWait) {
                            if (valtype == BsynValueType.Object) {
                                if (c == '"') {
                                    if (inTitle) inTitle = false;
                                    else {
                                        if (currentVarTitle.Length > 0) throw new Exception("To many \" in an objects title");
                                        inTitle = true;
                                    }
                                }

                                if (inTitle) {
                                    if (new Regex("[a-z0-9]", RegexOptions.IgnoreCase).IsMatch(c.ToString()))
                                        currentVarTitle += c;
                                } else if (c == ':') {
                                    typeWait = true;
                                }
                            } else if (valtype == BsynValueType.Array) {
                                currentVarTitle = bsynObjects.Count.ToString();
                                typeWait = true;
                            }
                        } else {
                            if (c == '{') {
                                type = 0;
                                inVar = true;
                            } else if (c == '[') {
                                type = 1;
                                inVar = true;
                            } else if (c == '"') {
                                type = 2;
                                inVar = true;
                                inQuotes = true;
                            } else if (new Regex("[0-9\"]").IsMatch(c.ToString())) {
                                type = 3;
                                inVar = true;
                                currentVarContent += c;
                            }
                        }
                    } else {
                        if (c == '"') inQuotes = !inQuotes;
                        switch (type) {
                            case 0: // Object
                                if (bracks <= 1) {
                                    bsynObjects.Add(new BsynObject('{' + currentVarContent + '}', currentVarTitle, BsynValueType.Object));
                                    type = -1;
                                    inVar = false;
                                    currentVarTitle = "";
                                    currentVarContent = "";
                                    typeWait = false;
                                    continue;
                                }
                                currentVarContent += c;
                                break;
                            case 1: // Array
                                if (bracks <= 1) {
                                    bsynObjects.Add(new BsynObject('[' + currentVarContent + ']', currentVarTitle, BsynValueType.Array));
                                    type = -1;
                                    inVar = false;
                                    currentVarTitle = "";
                                    currentVarContent = "";
                                    typeWait = false;
                                    continue;
                                }
                                currentVarContent += c;
                                break;
                            case 2: // String
                                if (c == '"') {
                                    bsynObjects.Add(new BsynObject(currentVarContent, currentVarTitle, BsynValueType.String));
                                    type = -1;
                                    inVar = false;
                                    currentVarTitle = "";
                                    currentVarContent = "";
                                    typeWait = false;
                                    continue;
                                }
                                currentVarContent += c;
                                break;
                            case 3: // Number
                                if (!new Regex("[0-9.]").IsMatch(c.ToString())) {
                                    bsynObjects.Add(new BsynObject(currentVarContent, currentVarTitle,
                                        (currentVarContent.Contains('.') ? BsynValueType.Float : BsynValueType.Int)
                                    ));
                                    type = -1;
                                    inVar = false;
                                    currentVarTitle = "";
                                    currentVarContent = "";
                                    typeWait = false;
                                    continue;
                                }
                                currentVarContent += c;
                                break;
                            default:
                                throw new Exception("Unknown object type");
                        }
                    }
                }
                if (bracks > 0) throw new Exception("To few '}', needs closing");
            }

            this.type = valtype;
        }

        public string Tree(int indent = 0) {
            string tabs = "";
            for (int i = 0; i < indent; i++) tabs += "\t";

            string t = "";

            if (type == BsynValueType.Array || type == BsynValueType.Object) {
                t += $"{tabs}{name}:\n";
                for (int i = 0; i < bsynObjects.Count; i++) {
                    BsynObject obj = bsynObjects[i];
                    t += (i > 0 ? "\n" : "") + obj.Tree(indent + 1);
                }
            } else {
                string p = (type == BsynValueType.String ? "\"" : "");
                t += $"{tabs}{name}: {p + value + p}";
            }
            return t;
        }

        BsynObject Get(string path) {
            //Path ex: Categories.Title
            BsynObject obj = this;
            foreach (string str in path.Split('.')) {
                bool found = false;
                foreach (BsynObject bo in obj.bsynObjects) {
                    if (bo.name == str) {
                        obj = bo;
                        found = true;
                        break;
                    }
                }
                if (!found) throw new Exception($"BsynObject could not be found on path '{path}'");
            }
            if (obj.name != path.Split('.')[path.Split('.').Length - 1]) throw new Exception($"BsynObject could not be found on path '{path}'");
            return obj;
        }

        public string GetString(string path) {
            BsynObject obj = Get(path);
            if (obj.type != BsynValueType.String) throw new Exception($"BsynObject at path '{path}' is not a string as expected");
            return obj.value;
        }

        public int GetInt(string path) {
            BsynObject obj = Get(path);
            if (obj.type != BsynValueType.Int) throw new Exception($"BsynObject at path '{path}' is not an int as expected");
            return int.Parse(obj.value);
        }

        public float GetFloat(string path) {
            BsynObject obj = Get(path);
            if (obj.type != BsynValueType.Float) throw new Exception($"BsynObject at path '{path}' is not a float as expected");
            return float.Parse(obj.value.Replace('.', ','));
        }

        public BsynObject GetObject(string path) {
            BsynObject obj = Get(path);
            if (obj.type != BsynValueType.Object) throw new Exception($"BsynObject at path '{path}' is not an object as expected");
            return obj;
        }

        public BsynObject[] GetArray(string path) {
            BsynObject obj = Get(path);
            if (obj.type != BsynValueType.Array) throw new Exception($"BsynObject at path '{path}' is not an array as expected");
            return obj.bsynObjects.ToArray();
        }

        public string[] GetStringArray(string path) {
            BsynObject obj = Get(path);
            if (obj.type != BsynValueType.Array) throw new Exception($"BsynObject at path '{path}' is not an array as expected");
            string[] arr = new string[obj.bsynObjects.Count];
            for (int i = 0; i < obj.bsynObjects.Count; i++) {
                BsynObject arobj = obj.bsynObjects[i];
                if (arobj.type != BsynValueType.String) throw new Exception($"BsynObject array object at path '{path}' is not a string as expected");
                arr[i] = arobj.value;
            }
            return arr;
        }

        public int[] GetIntArray(string path) {
            BsynObject obj = Get(path);
            if (obj.type != BsynValueType.Array) throw new Exception($"BsynObject at path '{path}' is not an array as expected");
            int[] arr = new int[obj.bsynObjects.Count];
            for (int i = 0; i < obj.bsynObjects.Count; i++) {
                BsynObject arobj = obj.bsynObjects[i];
                if (arobj.type != BsynValueType.Int) throw new Exception($"BsynObject array object at path '{path}' is not an int as expected");
                arr[i] = int.Parse(arobj.value);
            }
            return arr;
        }

        public float[] GetFloatArray(string path) {
            BsynObject obj = Get(path);
            if (obj.type != BsynValueType.Array) throw new Exception($"BsynObject at path '{path}' is not an array as expected");
            float[] arr = new float[obj.bsynObjects.Count];
            for (int i = 0; i < obj.bsynObjects.Count; i++) {
                BsynObject arobj = obj.bsynObjects[i];
                if (arobj.type != BsynValueType.Float) throw new Exception($"BsynObject array object at path '{path}' is not a float as expected");
                arr[i] = float.Parse(arobj.value.Replace('.', ','));
            }
            return arr;
        }

        public BsynObject[] GetObjectArray(string path) {
            BsynObject obj = Get(path);
            if (obj.type != BsynValueType.Array) throw new Exception($"BsynObject at path '{path}' is not an array as expected");
            BsynObject[] arr = new BsynObject[obj.bsynObjects.Count];
            for (int i = 0; i < obj.bsynObjects.Count; i++) {
                BsynObject arobj = obj.bsynObjects[i];
                if (arobj.type != BsynValueType.Object) throw new Exception($"BsynObject array object at path '{path}' is not an object as expected");
                arr[i] = arobj;
            }
            return arr;
        }
    }

    public enum BsynValueType {
        Object,
        Array,
        String,
        Int,
        Float
    }

    public static class BsynAddons {
        public static int Count(this string str, string search) {
            return str.Length - str.Replace(search, "").Length;
        }

        public static int Count(this string str, char search) {
            return str.Length - str.Replace(search.ToString(), "").Length;
        }

        public static string encodeToBsyn(this string str) {
            return str
                .Replace("\\\"", "&_c02");
        }

        public static string decodeToBsyn(this string str) {
            return str
                .Replace("&_c02", "\"");
        }

        public static string Replace(this string str, string[] arr, string replace) {
            foreach (string s in arr) str = str.Replace(s, replace);
            return str;
        }

        public static string Replace(this string str, char[] arr, string replace) {
            foreach (char s in arr) str = str.Replace(s.ToString(), replace);
            return str;
        }

        /// <summary>
        /// Checks if to strings are the same, but ignores the case
        /// </summary>
        /// <param name="check">The string to compare</param>
        public static bool EqualIgnoreCase(this string str, string check) {
            return (str.ToLower().Equals(check.ToLower()));
        }
    }
}
