using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

// Code by Maverick I. N.


public class Dictionary<TKey, TValue> : IDictionary<TKey, TValue>, IEnumerable<KeyValuePair<TKey, TValue>>
{
    LinkedList<KeyValuePair<TKey, TValue>>[] bucketList;
    private int count;
    private const int defaultSize = 16;
    public Dictionary(int size = defaultSize)
    {
        bucketList = new LinkedList<KeyValuePair<TKey, TValue>>[size];
        count = 0;
    }

    // Properties
    public int Count => count;

    public bool IsReadOnly => false;

    public ICollection<TKey> Keys   // Returns a list of all the keys in the KeyValuePairs
    {
        get
        {
            List<TKey> keys = new List<TKey>();
            foreach (var bucket in bucketList)
            {
                if (bucket != null)
                {
                    foreach (var pair in bucket)
                    {
                        keys.Add(pair.Key);
                    }
                }
            }
            return keys;
        }
        
    }

    public ICollection<TValue> Values   // Returns a list of all the values in the KeyValuePairs
    {
        get
        {
            List<TValue> values = new List<TValue>();
            foreach (var bucket in bucketList)
            {
                if (bucket != null)
                {
                    foreach (var pair in bucket)
                    {
                        values.Add(pair.Value);
                    }
                }
            }
            return values;
        }
    }

    public TValue this[TKey key]
    {
        get
        {
            if(TryGetValue(key, out TValue value))
            {
                return value;
            }
            throw new KeyNotFoundException();
        }

        set
        {
            if (ContainsKey(key))
            {
                Remove(key);
            }
            Add(key, value);
        }
    }

    // Methods
    
    public void Add(TKey key, TValue value)
    {   
        if(key == null || value == null)
        {
            if(key == null)
            {
                if(value == null)
                {
                    throw new ArgumentNullException("Both the key and the value is null");
                }
                throw new ArgumentNullException("The key is null");
            }
            else
            {
                throw new ArgumentNullException("The value is null");
            }
        }

        if (ContainsKey(key)) { throw new ArgumentException($"There is already the key: | {key} |"); }
        int index = GetBucketIndex(key);
        
        if (bucketList[index] == null)
        {
            bucketList[index] = new LinkedList<KeyValuePair<TKey, TValue>>();
        }
        bucketList[index].AddLast(new KeyValuePair <TKey, TValue >(key, value));
        count++;


    }
    public void Add(KeyValuePair<TKey, TValue> kvp) { Add(kvp.Key, kvp.Value); }
    public void Clear()
    {
        Array.Clear(bucketList);
        count = 0;
    }

    public bool Contains(KeyValuePair<TKey, TValue> kvp)
    {
        if(kvp.Value == null)
        {
            throw new ArgumentNullException($"The value of the KVP is null");
        }
        int index = GetBucketIndex(kvp.Key);
        if (bucketList[index] == null)
        {
            return false;
        }
        foreach (var pair in bucketList[index])
        {

        if(pair.Value != null)
            if (pair.Value.Equals(kvp.Value))
            {
                if (pair.Key != null)
                {
                    if (pair.Key.Equals(kvp.Key)) { return true; }
                }
            } 

        }
        return false;
    }

    public bool ContainsKey(TKey key)
    {
        if (key == null)
        {
            throw new ArgumentNullException("The key is null");
        }
        int index = GetBucketIndex(key);
        if (bucketList[index] == null)
        {
            return false;
        }
        foreach (var pair in bucketList[index])
        {
            if (pair.Key != null)
            {
                if (pair.Key.Equals(key)) { return true; }
            }

        }
        return false;
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        foreach (var bucket in bucketList)
        {
            if (bucket != null)
            {
                foreach (var pair in bucket)
                {
                    if (arrayIndex >= array.Length)
                    {
                        throw new ArgumentOutOfRangeException("Array is too small to use CopyTo");
                    }
                    array[arrayIndex++] = pair;
                }
            }
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        foreach (var bucket in bucketList)
        {
            if (bucket != null)
            {
                foreach (var pair in bucket)
                {
                    yield return pair;  // Returns all pairs from all buckets that arent null
                }
            }
        }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public bool Remove(TKey key)
    {
        if (ContainsKey(key))
        {
            int index = GetBucketIndex(key);
            
            foreach (var pair in bucketList[index])
            {
                if (pair.Key != null)
                {
                    if (pair.Key.Equals(key))
                    {
                        bucketList[index].Remove(pair);
                        count--;
                        return true;
                    }
                }

            }
        }
        return false;
    }
    public bool Remove(KeyValuePair<TKey, TValue> kvp)
    {
        if (Contains(kvp))
        {
            Remove(kvp.Key);
            return true;
        }
        return false;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (ContainsKey(key))
        {
            int index = GetBucketIndex(key);
            if (bucketList[index] != null)
            {
                foreach (var pair in bucketList[index])
                {
                    if (pair.Key != null)
                    {
                        if (pair.Key.Equals(key))
                        {
                            value = pair.Value;
                            return true;
                        }
                    }
                }
            }
        }
        value = default;    // Sets value to null since the key doesnt exist in the Hashtable, TryGetValue cant return a bool if "TValue value" isnt set to a value
        return false;
    }

    private int GetBucketIndex(TKey key)
    {
        if(key == null) { throw new ArgumentNullException(); }
        return Math.Abs(key.GetHashCode()) % bucketList.Length;
    }

}

class GeoLocation
{
    private double Latitude { get; }
    private double Longitude { get; }
   
    public GeoLocation(double latitude, double longitude)
    {
        if(latitude > 90 || latitude < -90)
        {
            throw new ArgumentOutOfRangeException();
        }
        if (longitude > 180 || longitude < -180)
        {
            throw new ArgumentOutOfRangeException();
        }

        Latitude = latitude;
        Longitude = longitude;
    }
    public override int GetHashCode()   // Cant get a hash code with two variables, overrides it to combine the two
    {
        return HashCode.Combine(Latitude, Longitude);
    }
    public override bool Equals(object? obj)    // Cant use Equals with GeoLocations since they are two values, overrides it so it checks if the object is a GeoLocation then checks if the latitudes and longitudes are the same
    {
        if (obj == null || !(obj is GeoLocation)) { return false; }
        GeoLocation otherObj = (GeoLocation)obj;
        return Latitude == otherObj.Latitude && Longitude == otherObj.Longitude;
    }
    public override string ToString()   // Overrides to display both Latitude and Longitude since they are seperate variables
    {
        return $"{Latitude}, {Longitude}";
    }
}

public class Client
{
    public static void Main()
    {
        //Test(new Dictionary<char, double>()); //to be replaced with your class (test for Task#1 provided in the assignment)

        // Start of test code for Task#2 provided by Jesper N.
        Dictionary<GeoLocation, string> places = new Dictionary<GeoLocation, string>();
        GeoLocation citySTHLM = new GeoLocation(59.334591, 18.063240);
        GeoLocation cityVSTRS = new GeoLocation(59.611366, 16.545025);
        places.Add(citySTHLM, "Stockholm");
        places.Add(cityVSTRS, "Västerås");
        // Defining GeoLocation instances for various cities
        GeoLocation cityLONDON = new GeoLocation(51.5074, -0.1278);   // London, UK
        GeoLocation cityPARIS = new GeoLocation(48.8566, 2.3522);     // Paris, France
        GeoLocation cityBERLIN = new GeoLocation(52.5200, 13.4050);   // Berlin, Germany
        GeoLocation cityMADRID = new GeoLocation(40.4168, -3.7038);   // Madrid, Spain
        GeoLocation cityNEWYORK = new GeoLocation(40.7128, -74.0060); // New York City, USA
        GeoLocation cityTOKYO = new GeoLocation(35.6762, 139.6503);   // Tokyo, Japan
        GeoLocation citySYDNEY = new GeoLocation(-33.8688, 151.2093); // Sydney, Australia
        GeoLocation cityRIO = new GeoLocation(-22.9068, -43.1729);    // Rio de Janeiro, Brazil
        GeoLocation cityCAPE = new GeoLocation(-33.9249, 18.4241);   // Cape Town, South Africa
        GeoLocation cityLOSANGELES = new GeoLocation(34.0522, -118.2437); // Los Angeles, USA
        GeoLocation cityTORONTO = new GeoLocation(43.65107, -79.347015); // Toronto, Canada
        GeoLocation cityBEIJING = new GeoLocation(39.9042, 116.4074);  // Beijing, China



        places.Add(cityLONDON, "London");
        places.Add(cityPARIS, "Paris");
        places.Add(cityBERLIN, "Berlin");
        places.Add(cityMADRID, "Madrid");
        places.Add(cityNEWYORK, "New York City");
        places.Add(cityTOKYO, "Tokyo");
        places.Add(citySYDNEY, "Sydney");
        places.Add(cityRIO, "Rio de Janeiro");
        places.Add(cityCAPE, "Cape Town");
        places.Add(cityLOSANGELES, "Los Angeles");
        places.Add(cityTORONTO, "Toronto");
        places.Add(cityBEIJING, "Beijing");
        Console.ForegroundColor = ConsoleColor.Blue;

        Console.WriteLine(places.Count);
        Console.ForegroundColor = ConsoleColor.Gray;
        foreach (var pair in places) { Console.WriteLine("Key: '{0}', Value: {1}", pair.Key, pair.Value); }

        try
        {
            places.Add(cityRIO, "asdklasjod");
        }
        catch
        (Exception e)
        {
            Console.WriteLine(e.GetType().ToString() + " : " + e.Message);
        }
        Console.WriteLine(places.Remove(cityRIO));
        places.Add(cityRIO, "asdklasjod");
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(places.Count);
        Console.ForegroundColor = ConsoleColor.Gray;
        foreach (var pair in places) { Console.WriteLine("Key: '{0}', Value: {1}", pair.Key, pair.Value); }
        places.Clear();

        Console.WriteLine(places.Count);
        // End of test code for Task#2
    }
    static public void Test(IDictionary<char, double> d)
    {
        //d.Add('?', 3.9);
        //d.Add('2', 0.2);
        //d.Add('r', 1.2);
        //var p = new KeyValuePair<char, double>('*', 5.2);
        //d.Add(p);
        //d['?'] -= d['?'];
        //try { d.Add('2', 6.0); } catch (Exception e) { Console.WriteLine(e.GetType().ToString() + " : " + e.Message); }
        //// System.ArgumentException : An item with the same key has already been added. Key: 2
        //foreach (var elem in d.Keys)
        //    Console.Write("'{0}' ", elem);
        //Console.WriteLine();
        //// '?' '2' 'r' '*'
        //foreach (var elem in d.Values)
        //    Console.Write("{0} ", elem);
        //Console.WriteLine();
        //// 0 0.2 1.2 5.2 
        //int count = 0;
        //foreach (var elem in d)
        //{ Console.Write("{0},'{1}' ", elem.Key, elem.Value); ++count; }
        //Console.WriteLine("(" + d.Count + "=" + count + ")");
        //// ?,'0' 2,'0.2' r,'1.2' *,'5.2' (4=4)
        //Console.WriteLine("{0} {1}", d.ContainsKey('2'), d.ContainsKey('>'));
        //// True False
        //Console.WriteLine("{0} {1}", d.Contains(new KeyValuePair<char, double>('r', 7.9)), d.Contains(p));
        //// False True
        //KeyValuePair<char, double>[] a = new KeyValuePair<char, double>[d.Count];
        //try { d.CopyTo(a, 1); } catch (Exception e) { Console.WriteLine(e.GetType().ToString() + " : " + e.Message); }
        //// System.ArgumentException : Destination array is not long enough to copy all the items in the collection. Check array index and length.
        //d.CopyTo(a, 0);
        //foreach (var elem in a)
        //    Console.Write("'{0}'={1} ", elem.Key, d.ContainsKey(elem.Key));
        //Console.WriteLine();
        ////'?'=True '2'=True 'r'=True '*'=True
        //Console.WriteLine("{0} {1}", d.Remove('?'), d.Remove('#'));
        //// True False
        //Console.WriteLine(d.Count);
        //// 3
        //Console.WriteLine("{0} {1}", d.Remove(new KeyValuePair<char, double>('r', 7.9)), d.Remove(p));
        //// False True
        //Console.WriteLine(d.Count);
        //// 2
        //double v1 = -1, v2 = -1;
        //d.TryGetValue('r', out v1);
        //d.TryGetValue('R', out v2);
        //Console.WriteLine(v1 + " " + v2);
        //// 1.2 0
        //d.Clear();
        //count = 0;
        //foreach (var elem in d) ++count;
        //Console.WriteLine(d.Count + "=" + count);
        //// 0=0
    }
}