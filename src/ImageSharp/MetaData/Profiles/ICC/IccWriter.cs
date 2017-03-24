﻿// <copyright file="IccWriter.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Contains methods for writing ICC profiles.
    /// </summary>
    internal sealed class IccWriter
    {
        /// <summary>
        /// Writes the ICC profile into a byte array
        /// </summary>
        /// <param name="profile">The ICC profile to write</param>
        /// <returns>The ICC profile as a byte array</returns>
        public byte[] Write(IccProfile profile)
        {
            Guard.NotNull(profile, nameof(profile));

            IccDataWriter writer = new IccDataWriter();
            IccTagTableEntry[] tagTable = this.WriteTagData(writer, profile.Entries);
            this.WriteTagTable(writer, tagTable);
            this.WriteHeader(writer, profile.Header);
            return writer.GetData();
        }

        private void WriteHeader(IccDataWriter writer, IccProfileHeader header)
        {
            writer.SetIndex(0);

            writer.WriteUInt32(writer.Length + 128);
            writer.WriteAsciiString(header.CmmType, 4, false);
            writer.WriteVersionNumber(header.Version);
            writer.WriteUInt32((uint)header.Class);
            writer.WriteUInt32((uint)header.DataColorSpace);
            writer.WriteUInt32((uint)header.ProfileConnectionSpace);
            writer.WriteDateTime(header.CreationDate);
            writer.WriteAsciiString("acsp");
            writer.WriteUInt32((uint)header.PrimaryPlatformSignature);
            writer.WriteInt32((int)header.Flags);
            writer.WriteUInt32(header.DeviceManufacturer);
            writer.WriteUInt32(header.DeviceModel);
            writer.WriteInt64((long)header.DeviceAttributes);
            writer.WriteUInt32((uint)header.RenderingIntent);
            writer.WriteXyzNumber(header.PcsIlluminant);
            writer.WriteAsciiString(header.CreatorSignature, 4, false);

#if !NETSTANDARD1_1
            IccProfileId id = IccProfile.CalculateHash(writer.GetData());
            writer.WriteProfileId(id);
#else
            writer.WriteProfileId(IccProfileId.Zero);
#endif
        }

        private void WriteTagTable(IccDataWriter writer, IccTagTableEntry[] table)
        {
            // 128 = size of ICC header
            writer.SetIndex(128);

            writer.WriteUInt32((uint)table.Length);
            foreach (IccTagTableEntry entry in table)
            {
                writer.WriteUInt32((uint)entry.Signature);
                writer.WriteUInt32(entry.Offset);
                writer.WriteUInt32(entry.DataSize);
            }
        }

        private IccTagTableEntry[] WriteTagData(IccDataWriter writer, List<IccTagDataEntry> entries)
        {
            List<IccTagDataEntry> inData = new List<IccTagDataEntry>(entries);
            List<IccTagDataEntry[]> dupData = new List<IccTagDataEntry[]>();

            // Filter out duplicate entries. They only need to be defined once but can be used multiple times
            while (inData.Count > 0)
            {
                IccTagDataEntry[] items = inData.Where(t => inData[0].Equals(t)).ToArray();
                dupData.Add(items);
                foreach (IccTagDataEntry item in items)
                {
                    inData.Remove(item);
                }
            }

            List<IccTagTableEntry> table = new List<IccTagTableEntry>();

            // (Header size) + (entry count) + (nr of entries) * (size of table entry)
            writer.SetIndex(128 + 4 + (entries.Count * 12));

            foreach (IccTagDataEntry[] entry in dupData)
            {
                writer.WriteTagDataEntry(entry[0], out IccTagTableEntry tentry);
                foreach (IccTagDataEntry item in entry)
                {
                    table.Add(new IccTagTableEntry(item.TagSignature, tentry.Offset, tentry.DataSize));
                }
            }

            return table.ToArray();
        }
    }
}