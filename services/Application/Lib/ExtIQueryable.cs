using Application.Lib;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Application.Lib;

public static class ExtIQueryable
{
    public static async Task<PageData> Page<T>(this IQueryable<T> q, int skip, int take, CancellationToken ct)
    {
        try
        {
            var st = new StackTrace(true);
            var frames = st.GetFrames()
                .Select(x =>
                {
                    var m = x.GetMethod();
                    return new
                    {
                        IsAsync = m.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) != null,
                        Method = m.Name,
                        Method2 = m,
                        FileName = x.GetFileName(),
                        LineNumber = x.GetFileLineNumber(),
                        ColumnNumber = x.GetFileColumnNumber(),
                    };
                })
                .ToList();
            var mainLines = new List<object>();
            for (int i = 0; i < frames.Count; i++)
            {
                if (frames[i] is { LineNumber: 0, ColumnNumber: 0 })
                    continue;
                if (frames[i] is { LineNumber: > 0, ColumnNumber: > 0, Method: "MoveNext", IsAsync: false })
                {
                    var i2 = i + 1;
                    while (frames[i2] is { Method: "Start", IsAsync: false, LineNumber: 0, ColumnNumber: 0 }) i2++;

                    mainLines.Add(new
                    {
                        frames[i2].IsAsync,
                        frames[i2].Method,
                        frames[i].FileName,
                        frames[i].LineNumber
                    });
                    i = i2;
                }
                else
                {
                    mainLines.Add(frames[i]);
                }
            }
         
            //var sql = q.ToQueryString();
            //var sql2 = q.Skip(skip).Take(take).ToQueryString();
            var count = await q.CountAsync(ct).ConfigureAwait(false);
            return new(
               count == 0
                    ? []
                    : await q.Skip(skip).Take(take).ToListAsync(ct).ConfigureAwait(false),
               count);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    public static async Task<PageData<T>> Page2<T>(this IQueryable<T> q, int skip, int take, CancellationToken ct)
    {
        try
        {
            //var sql = q.ToQueryString();
            //var sql2 = q.Skip(skip).Take(take).ToQueryString();
            var count = await q.CountAsync(ct).ConfigureAwait(false);
            return new(
               count == 0
                    ? []
                    : await q.Skip(skip).Take(take).ToListAsync(ct).ConfigureAwait(false),
               count);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public static async Task<PageData> Page<T>(this IOrderedQueryable<T> q, int skip, int take, CancellationToken ct)
       => await q.AsQueryable().Page(skip, take, ct);

    public static async Task<PageData> Page<T>(this IOrderedEnumerable<T> q, int skip, int take, CancellationToken ct)
       => await q.AsQueryable().Page(skip, take, ct);

    public static PageData Page<T>(this IList<T> q, int skip, int take)
        => new(q.Skip(skip).Take(take).ToList(), q.Count);


    public static IOrderedQueryable<TSource> OrderBy<TSource, TKey>(this IQueryable<TSource> q,
                            Expression<Func<TSource, TKey>> keySelector, string orderType)
    => orderType.Trim().Equals("asc", StringComparison.OrdinalIgnoreCase) ? q.OrderBy(keySelector) : q.OrderByDescending(keySelector);

    public static IQueryable<T> OrderBy<T>(this List<T> query, string propertyName, string orderType)
    {
        return query.AsQueryable().OrderBy(propertyName, orderType);
    }

    public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string propertyName, string orderType)
    {
        if (string.IsNullOrEmpty(propertyName))
            return query;

        var orderByMethod = orderType.Equals("DESC", StringComparison.OrdinalIgnoreCase) ? "OrderByDescending" : "OrderBy";

        ParameterExpression pe = Expression.Parameter(query.ElementType);
        MemberExpression me = Expression.Property(pe, propertyName);

        MethodCallExpression orderByCall = Expression.Call(typeof(Queryable), orderByMethod, new Type[] { query.ElementType, me.Type }, query.Expression
            , Expression.Quote(Expression.Lambda(me, pe)));

        return query.Provider.CreateQuery(orderByCall) as IQueryable<T>;
    }
    public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string propertyName)
       => query.OrderBy(propertyName, "ASC");
    public static IQueryable<T> OrderByDescending<T>(this IQueryable<T> query, string propertyName)
        => query.OrderBy(propertyName, "DESC");


}


public record PageData
{
    public IEnumerable Data { get; private set; }
    public int Total { get; private set; }
    public int PageNO { get; set; }
    public object ExtraData { get; set; }
    public object Errors { get; set; }

    public PageData(IEnumerable data, int total)
    {
        Data = data;
        Total = total;
    }
}
public record PageData<T>
{
    public IEnumerable<T> Data { get; private set; }
    public int Total { get; private set; }
    public int PageNO { get; set; }
    public object ExtraData { get; set; }
    public object Errors { get; set; }

    public PageData(IEnumerable<T> data, int total)
    {
        Data = data;
        Total = total;
    }
}