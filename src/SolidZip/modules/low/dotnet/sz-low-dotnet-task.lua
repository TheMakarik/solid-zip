local task = {};

function task.async(callback)
    return function(...)
        local co = coroutine.create(callback)
        coroutine.resume(co, ...)
    end
end

function task.await(dotnetTask)
    local co = coroutine.running()

    dotnetTask:ContinueWith(function(completedTask)
        coroutine.resume(co, completedTask)
    end)

    return coroutine.yield()
end

return task;