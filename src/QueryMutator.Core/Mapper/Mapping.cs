﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace QueryMutator.Core
{
    public interface IMapping
    {
        LambdaExpression Expression { get; }
    }

    public interface IMapping<TSource, TTarget> : IMapping
    {
        new Expression<Func<TSource, TTarget>> Expression { get; }
    }

    internal class Mapping<TSource, TTarget> : IMapping<TSource, TTarget>
    {
        public Expression<Func<TSource, TTarget>> Expression { get; set; }

        LambdaExpression IMapping.Expression => Expression;
    }

    public interface IMapping<TSource, TTarget, TParam>
    {
    }

    internal class Mapping<TSource, TTarget, TParam> : IMapping, IMapping<TSource, TTarget, TParam>
    {
        public MappingBuilder<TSource, TTarget, TParam> Builder { get; set; }

        LambdaExpression IMapping.Expression => null;
    }
}
