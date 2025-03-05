#include "template_engine.h"

#include <inja/inja.hpp>
#include <nlohmann/json.hpp>

#include "stream_wrapper.hpp"

namespace  {
inja::Environment* GetEnvironment(const TemplateEngineEnvironment& environment) {
    return reinterpret_cast<inja::Environment*>(environment.ptr);
}
}

TemplateEngineEnvironment AllocTemplateEngineEnvironment() {
    auto ret = TemplateEngineEnvironment();
    ret.ptr = new inja::Environment();
    return ret;
}

void FreeTemplateEngineEnvironment(TemplateEngineEnvironment environment) {
    delete GetEnvironment(environment);
}

void RenderTemplateToStream(IoStream stream, TemplateEngineEnvironment environment, const char* template_text, const char* json_data) {
    try {
        auto s = GetStream(stream)->stream();
        auto env = GetEnvironment(environment);
        auto data = nlohmann::json::parse(json_data);
        env->render_to(*s, env->parse(template_text), data);
    }
    catch (std::exception e) {
        
    }
}
