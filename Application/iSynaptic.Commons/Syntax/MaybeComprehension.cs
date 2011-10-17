﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace iSynaptic.Commons.Syntax
{
    public static class MaybeComprehension
    {
        public static Maybe<TResult> SelectMany<T, TResult>(this Maybe<T> @this, Func<T, Maybe<TResult>> selector)
        {
            return @this.Bind(selector);
        }

        public static Maybe<TResult> SelectMany<T, TIntermediate, TResult>(this Maybe<T> @this, Func<T, Maybe<TIntermediate>> selector, Func<T, TIntermediate, TResult> combiner)
        {
            Guard.NotNull(selector, "selector");
            Guard.NotNull(combiner, "combiner");

            var self = @this;
            return new Maybe<TResult>(() =>
            {
                if (self.Exception != null)
                    return new Maybe<TResult>(self.Exception);

                if (self.HasValue != true)
                    return Maybe<TResult>.NoValue;

                var value = self.Value;
                var intermediate = selector(value);

                if (intermediate.Exception != null)
                    return new Maybe<TResult>(intermediate.Exception);

                return intermediate.HasValue ? new Maybe<TResult>(combiner(value, intermediate.Value)) : Maybe<TResult>.NoValue;
            });
        }
    }
}
