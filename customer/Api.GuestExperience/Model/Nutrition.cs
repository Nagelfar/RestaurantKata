/*
 * Guest experience
 *
 * Provides a menu with daily offers tailored to customers, which is compliant to the legal rules of the country. 
 *
 * The version of the OpenAPI document: 1.0.0
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */


using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace Api.GuestExperience.Model
{
    /// <summary>
    /// Contains the nutrition information according to Austrian law, see &lt;https://www.wko.at/branchen/tourismus-freizeitwirtschaft/gastronomie/weiterfuehrende_infos_allergene.html&gt; 
    /// </summary>
    /// <value>Contains the nutrition information according to Austrian law, see &lt;https://www.wko.at/branchen/tourismus-freizeitwirtschaft/gastronomie/weiterfuehrende_infos_allergene.html&gt; </value>
    
    [JsonConverter(typeof(StringEnumConverter))]
    
    public enum Nutrition
    {
        /// <summary>
        /// Enum A for value: A
        /// </summary>
        [EnumMember(Value = "A")]
        A = 1,

        /// <summary>
        /// Enum B for value: B
        /// </summary>
        [EnumMember(Value = "B")]
        B = 2,

        /// <summary>
        /// Enum C for value: C
        /// </summary>
        [EnumMember(Value = "C")]
        C = 3,

        /// <summary>
        /// Enum D for value: D
        /// </summary>
        [EnumMember(Value = "D")]
        D = 4,

        /// <summary>
        /// Enum E for value: E
        /// </summary>
        [EnumMember(Value = "E")]
        E = 5,

        /// <summary>
        /// Enum F for value: F
        /// </summary>
        [EnumMember(Value = "F")]
        F = 6,

        /// <summary>
        /// Enum G for value: G
        /// </summary>
        [EnumMember(Value = "G")]
        G = 7,

        /// <summary>
        /// Enum H for value: H
        /// </summary>
        [EnumMember(Value = "H")]
        H = 8,

        /// <summary>
        /// Enum L for value: L
        /// </summary>
        [EnumMember(Value = "L")]
        L = 9,

        /// <summary>
        /// Enum M for value: M
        /// </summary>
        [EnumMember(Value = "M")]
        M = 10,

        /// <summary>
        /// Enum N for value: N
        /// </summary>
        [EnumMember(Value = "N")]
        N = 11,

        /// <summary>
        /// Enum O for value: O
        /// </summary>
        [EnumMember(Value = "O")]
        O = 12,

        /// <summary>
        /// Enum P for value: P
        /// </summary>
        [EnumMember(Value = "P")]
        P = 13,

        /// <summary>
        /// Enum R for value: R
        /// </summary>
        [EnumMember(Value = "R")]
        R = 14

    }

}
