﻿#include "pch.h"

BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

MIB_IPFORWARD_ROW2 GetRoute(const char* address, int netmask, const char* gateway, int index, int metric)
{
	MIB_IPFORWARD_ROW2 row = { 0 };
	row.InterfaceIndex = index;
	row.DestinationPrefix.Prefix.si_family = row.NextHop.si_family = AF_INET;
	row.DestinationPrefix.Prefix.Ipv4.sin_addr.S_un.S_addr = inet_addr(address);
	row.DestinationPrefix.PrefixLength = netmask;
	row.NextHop.Ipv4.sin_addr.S_un.S_addr = inet_addr(gateway);
	row.ValidLifetime = 0xffffffff;
	row.PreferredLifetime = 0xffffffff;
	row.Metric = metric;
	row.Protocol = MIB_IPPROTO_OTHER;

	return row;
}

// 创建路由规则
DLLEXPORT BOOL CreateRoute(const char* address, int netmask, const char* gateway, int index, int metric = 0)
{
	return (CreateIpForwardEntry2(&GetRoute(address, netmask, gateway, index, metric)) == NO_ERROR) ? TRUE : FALSE;
}

// 修改路由规则
DLLEXPORT BOOL ChangeRoute(const char* address, int netmask, const char* gateway, int index, int metric = 0)
{
	return (SetIpForwardEntry2(&GetRoute(address, netmask, gateway, index, metric)) == NO_ERROR) ? TRUE : FALSE;
}

// 删除路由规则
DLLEXPORT BOOL DeleteRoute(const char* address, int netmask, const char* gateway, int index, int metric = 0)
{
	return (DeleteIpForwardEntry2(&GetRoute(address, netmask, gateway, index, -1)) == NO_ERROR) ? TRUE : FALSE;
}