#pragma once
#include "global.h"
#include "generic.h"

struct TemplateEngineEnvironment {
    void* ptr;
};

EXPORT TemplateEngineEnvironment AllocTemplateEngineEnvironment();
EXPORT void FreeTemplateEngineEnvironment(TemplateEngineEnvironment environment);

EXPORT void RenderTemplateToStream(IoStream stream, TemplateEngineEnvironment environment, const char* template_text, const char* json_data);
