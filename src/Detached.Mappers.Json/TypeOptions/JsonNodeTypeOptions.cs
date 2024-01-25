﻿using Detached.Mappers.Annotations;
using Detached.Mappers.Types;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.Json.Nodes;

namespace Detached.Mappers.Json.TypeOptions
{
    public class JsonNodeTypeOptions : IType
    {
        public JsonNodeTypeOptions()
        {
            this.Abstract(true);
        }

        public Type ClrType => typeof(JsonNode);

        public Type ItemClrType => default;

        public AnnotationCollection Annotations { get; } = new();

        public MappingSchema MappingSchema => MappingSchema.None;

        public IEnumerable<string> MemberNames => new string[0]; 

        public Expression BuildNewExpression(Expression context, Expression discriminator)
        {
            throw new NotImplementedException();
        }

        public ITypeMember GetMember(string memberName)
        {
            return new JsonObjectMemberOptions(memberName);
        }
    }
}
