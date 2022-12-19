﻿using Application.Contracts.Model;
using AutoMapper;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Domain.Model;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Test.Support;

//https://github.com/dotnet/BenchmarkDotNet

namespace Test.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class RepositoryBenchmarks
{
    //Infrastructure
    private readonly IMapper _mapper;
    private readonly ITodoRepository _repo;

    public RepositoryBenchmarks()
    {
        _mapper = SampleApp.Bootstrapper.Automapper.ConfigureAutomapper.CreateMapper();
        TodoContext db = new InMemoryDbBuilder()
            .SeedDefaultEntityData()
            .UseEntityData(entities =>
            {
                //custom data scenario that default seed data does not cover
                entities.Add(new TodoItem { Id = Guid.NewGuid(), Name = "some entity", CreatedBy = "unit test", UpdatedBy = "unit test", CreatedDate = DateTime.UtcNow });
            })
            .GetOrBuild<TodoContext>();

        _repo = new TodoRepository(db, _mapper);
    }

    [IterationSetup]
    public void Setup()
    {
        _ = GetHashCode();
    }

    [Benchmark]
    public async Task Repo_PagedTodoDtoResponse()
    {
        //act & assert
        _ = await _repo.GetDtosPagedAsync(10, 1);
    }

    [Benchmark]
    public async Task Repo_PagedTodoResponse()
    {
        //act & assert
        _ = await _repo.GetItemsPagedAsync(10, 1);
    }


}