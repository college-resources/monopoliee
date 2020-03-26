using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UniRx;

namespace Schema
{
    public class Property : IDisposable
    {
        #region Caching

        private static Dictionary<string, Property> _properties;

        public static Property GetProperty(JToken property)
        {
            if (_properties == null)
            {
                _properties = new Dictionary<string, Property>();
            }

            var propertyId = (string) property["_id"];

            if (!_properties.ContainsKey(propertyId))
            {
                var newProperty = new Property(property);
                _properties.Add(propertyId, newProperty);
            }

            return _properties[propertyId];
        }

        public static Property GetPropertyByLocation(int location)
        {
            foreach (var property in _properties)
            {
                if (property.Value.Location == location)
                    return property.Value;
            }
            return null;
        }

        #endregion

        private readonly string _id;
        private readonly string _name;
        private bool _mortgaged;

        public string Name => _name;
        public BehaviorSubject<string> OwnerId { get; }
        public bool Mortgaged => _mortgaged;
        public int Location { get; }

        private Property(JToken property)
        {
            _id = (string) property["_id"];
            _name = (string) property["name"];
            OwnerId = new BehaviorSubject<string> ((string) property["owner"]);
            _mortgaged = (bool) property["mortgaged"];
            Location = (int) property["location"];
        }

        public void Dispose()
        {
            OwnerId?.Dispose();
            
            _properties.Remove(_id);
            
            GC.SuppressFinalize(this);
        }
    }
}