#include <utils.h>
#include <fstream>
#include <vector>
#include "CSharpScriptRuntime.h"
#include "CSharpResourceImpl.h"
#include <Log.h>
#include "CRC.h"
#include <sstream>
#include <string>
#include <iomanip>
#include "natives.h"

using namespace std;

bool CSharpResourceImpl::Start()
{
    Log::Info << "Starting resource" << Log::Endl;
    runtime->clr.Initialize();
    resource->EnableNatives();
    auto scope = resource->PushNativesScope();
    ResetDelegates();
    CoreCLR::StartResource(resource, core);
    return true;
}

bool CSharpResourceImpl::Stop()
{
    auto scope = resource->PushNativesScope();
    GetRuntime()->clr.StopResource(resource);
    ResetDelegates();
    return true;
}

bool CSharpResourceImpl::OnEvent(const alt::CEvent* ev)
{
    if (ev == nullptr) return true;
    auto scope = resource->PushNativesScope();
    switch(ev->GetType()) {
        case alt::CEvent::Type::SERVER_SCRIPT_EVENT: {
            auto serverScriptEvent = (alt::CServerScriptEvent *) ev;
            alt::MValueArgs serverArgs = serverScriptEvent->GetArgs();
            uint64_t size = serverArgs.GetSize();
            if (size == 0) {
                OnServerEventDelegate(serverScriptEvent->GetName().c_str(), nullptr, 0);
            } else {
                auto constArgs = new alt::MValueConst *[size];
                for (uint64_t i = 0; i < size; i++) {
                    constArgs[i] = &serverArgs[i];
                }
                OnServerEventDelegate(serverScriptEvent->GetName().c_str(), constArgs, size);
                delete[] constArgs;
            }
            break;
        }
        case alt::CEvent::Type::CLIENT_SCRIPT_EVENT: {
            auto clientScriptEvent = (alt::CClientScriptEvent *) ev;
            alt::MValueArgs serverArgs = clientScriptEvent->GetArgs();
            uint64_t size = serverArgs.GetSize();
            if (size == 0) {
                OnClientEventDelegate(clientScriptEvent->GetName().c_str(), nullptr, 0);
            } else {
                auto constArgs = new alt::MValueConst *[size];
                for (uint64_t i = 0; i < size; i++) {
                    constArgs[i] = &serverArgs[i];
                }
                OnClientEventDelegate(clientScriptEvent->GetName().c_str(), constArgs, size);
                delete[] constArgs;
            }
            break;
        }
        case alt::CEvent::Type::CONSOLE_COMMAND_EVENT: {
            auto consoleCommandEvent = (alt::CConsoleCommandEvent *) ev;
            auto args = consoleCommandEvent->GetArgs();
            uint64_t size = args.size();
            auto cArgs = new const char *[size];
            for (uint64_t i = 0; i < size; i++) {
                cArgs[i] = args[i].c_str();
            }
            auto name = consoleCommandEvent->GetName();
            OnConsoleCommandDelegate(name.c_str(), cArgs, (uint32_t) size);
            delete[] cArgs;
            break;
        }
        case alt::CEvent::Type::WEB_VIEW_EVENT: {
            auto webViewEvent = (alt::CWebViewEvent *) ev;
            auto args = webViewEvent->GetArgs();
            auto name = webViewEvent->GetName();
            auto size = args.GetSize();
            auto constArgs = new alt::MValueConst *[size];

            for (auto i = 0; i < size; i++) {
                constArgs[i] = &args[i];
            }
            OnWebViewEventDelegate(webViewEvent->GetTarget().Get(), name.c_str(), constArgs, size);
            delete[] constArgs;
            break;
        }
#pragma region Player Events
        case alt::CEvent::Type::SPAWNED: {
            OnPlayerSpawnDelegate();
            break;
        }
        case alt::CEvent::Type::DISCONNECT_EVENT: {
            OnPlayerDisconnectDelegate();
            break;
        }
        case alt::CEvent::Type::PLAYER_ENTER_VEHICLE: {
            auto playerEnterVehicleEvent = (alt::CPlayerEnterVehicleEvent *) ev;
            OnPlayerEnterVehicleDelegate(playerEnterVehicleEvent->GetTarget().Get(),
                                         playerEnterVehicleEvent->GetSeat());
            break;
        }
#pragma endregion
#pragma region Entity events
        case alt::CEvent::Type::GAME_ENTITY_CREATE: {
            auto gameEntityCreateEvent = (alt::CGameEntityCreateEvent *) ev;
            auto entity = gameEntityCreateEvent->GetTarget().Get();
            auto type = (uint8_t) entity->GetType();
            void *ptr;

            switch (entity->GetType()) {
                case alt::IBaseObject::Type::PLAYER:
                    ptr = dynamic_cast<alt::IPlayer *>(entity);
                    break;
                case alt::IBaseObject::Type::VEHICLE:
                    ptr = dynamic_cast<alt::IVehicle *>(entity);
                    break;
                default:
                    ptr = nullptr;
            }

            OnGameEntityCreateDelegate(ptr, type);
            break;
        }
        case alt::CEvent::Type::GAME_ENTITY_DESTROY: {
            auto gameEntityDestroyEvent = (alt::CGameEntityDestroyEvent *) ev;
            auto entity = gameEntityDestroyEvent->GetTarget().Get();
            auto type = (uint8_t) entity->GetType();
            void *ptr;

            switch (entity->GetType()) {
                case alt::IBaseObject::Type::PLAYER:
                    ptr = dynamic_cast<alt::IPlayer *>(entity);
                    break;
                case alt::IBaseObject::Type::VEHICLE:
                    ptr = dynamic_cast<alt::IVehicle *>(entity);
                    break;
                default:
                    ptr = nullptr;
            }

            OnGameEntityDestroyDelegate(ptr, type);
            break;
        }
#pragma endregion
#pragma region Misc
        case alt::CEvent::Type::RESOURCE_ERROR: {
            auto resourceErrorEvent = (alt::CResourceErrorEvent *) ev;
            OnResourceErrorDelegate(resourceErrorEvent->GetResource()->GetName().c_str());
            break;
        }
        case alt::CEvent::Type::RESOURCE_START: {
            auto resourceStartEvent = (alt::CResourceStartEvent *) ev;
            OnResourceStartDelegate(resourceStartEvent->GetResource()->GetName().c_str());
            break;
        }
        case alt::CEvent::Type::RESOURCE_STOP: {
            auto resourceStopEvent = (alt::CResourceStopEvent *) ev;
            OnResourceStopDelegate(resourceStopEvent->GetResource()->GetName().c_str());
            break;
        }
        case alt::CEvent::Type::KEYBOARD_EVENT: {
            auto keyboardEvent = (alt::CKeyboardEvent *) ev;
            if (keyboardEvent->GetKeyState() == alt::CKeyboardEvent::KeyState::UP)
                OnKeyUpDelegate(keyboardEvent->GetKeyCode());
            else
                OnKeyDownDelegate(keyboardEvent->GetKeyCode());
            break;
        }
#pragma endregion
    }
    return true;
}

void CSharpResourceImpl::OnTick()
{
    OnTickDelegate();
}

void CSharpResourceImpl::OnCreateBaseObject(alt::Ref<alt::IBaseObject> objectRef)
{
    auto object = objectRef.Get();
    if (object == nullptr) return;

    switch (object->GetType()) {
        case alt::IBaseObject::Type::VEHICLE:
        {
            auto vehicle = dynamic_cast<alt::IVehicle*>(object);
            OnCreateVehicleDelegate(vehicle, vehicle->GetID());
            break;
        }
        case alt::IBaseObject::Type::PLAYER:
        {
            auto player = dynamic_cast<alt::IPlayer*>(object);
            OnCreatePlayerDelegate(player, player->GetID());
            break;
        }
    }
}

void CSharpResourceImpl::OnRemoveBaseObject(alt::Ref<alt::IBaseObject> objectRef)
{
    auto object = objectRef.Get();
    if (object == nullptr) return;

    switch (object->GetType()) {
        case alt::IBaseObject::Type::VEHICLE:
        {
            OnRemoveVehicleDelegate(dynamic_cast<alt::IVehicle*>(object));
            break;
        }
        case alt::IBaseObject::Type::PLAYER:
        {
            OnRemovePlayerDelegate(dynamic_cast<alt::IPlayer*>(object));
            break;
        }
    }
}

std::string CSharpResourceImpl::ReadFile(std::string path)
{
    auto pkg = resource->GetPackage();
    if(!pkg->FileExists(path)) return {};
    alt::IPackage::File* pkgFile = pkg->OpenFile(path);
    std::string src(pkg->GetFileSize(pkgFile), 0);
    pkg->ReadFile(pkgFile, src.data(), src.size());
    pkg->CloseFile(pkgFile);

    return src;
}


void CSharpResourceImpl::ResetDelegates() {
    OnTickDelegate = []() {};
    OnClientEventDelegate = [](auto var, auto var2, auto var3) {};
    OnServerEventDelegate = [](auto var, auto var2, auto var3) {};
    OnWebViewEventDelegate = [](auto var, auto var2, auto var3, auto var4) {};
    OnConsoleCommandDelegate = [](auto var, auto var2, auto var3) {};

    OnCreatePlayerDelegate = [](auto var, auto var2) {};
    OnRemovePlayerDelegate = [](auto var) {};

    OnCreateVehicleDelegate = [](auto var, auto var2) {};
    OnRemoveVehicleDelegate = [](auto var) {};

    OnPlayerSpawnDelegate = [](){};
    OnPlayerDisconnectDelegate = [](){};
    OnPlayerEnterVehicleDelegate = [](auto var, auto var2) {};

    OnGameEntityCreateDelegate = [](auto var, auto var2) {};
    OnGameEntityDestroyDelegate = [](auto var, auto var2) {};

    OnResourceErrorDelegate = [](auto var) {};
    OnResourceStartDelegate = [](auto var) {};
    OnResourceStopDelegate = [](auto var) {};

    OnKeyUpDelegate = [](auto var) {};
    OnKeyDownDelegate = [](auto var) {};
}