#include "template_engine.h"

#include <inja/inja.hpp>
#include <nlohmann/json.hpp>

#include "generic/stream_wrapper.hpp"

namespace {
    inja::Environment* GetEnvironment(const TemplateEngineEnvironment& environment) {
        return reinterpret_cast<inja::Environment*>(environment.ptr);
    }
}

TemplateEngineEnvironment AllocTemplateEngineEnvironment() {
    TRY_CATCH_BEGIN
        auto ret = TemplateEngineEnvironment();
        ret.ptr = new inja::Environment();
        return ret;
    TRY_CATCH_END
    return {};
}

void FreeTemplateEngineEnvironment(TemplateEngineEnvironment environment) {
    TRY_CATCH_BEGIN
        delete GetEnvironment(environment);
    TRY_CATCH_END
}

void RenderTemplateToStream(IoStream stream, TemplateEngineEnvironment environment, const char* template_text,
                            const char* json_data) {
    TRY_CATCH_BEGIN
        auto s = GetStream(stream)->stream();
        auto env = GetEnvironment(environment);
        auto data = nlohmann::json::parse(json_data ? json_data : "");
        env->render_to(*s, env->parse(template_text ? template_text : ""), data);
    TRY_CATCH_END
}
