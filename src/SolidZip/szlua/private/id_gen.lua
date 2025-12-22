local id_gen = {}

function id_gen.generate()
    import("System")
    return Guid.NewGuid()
end

return id_gen