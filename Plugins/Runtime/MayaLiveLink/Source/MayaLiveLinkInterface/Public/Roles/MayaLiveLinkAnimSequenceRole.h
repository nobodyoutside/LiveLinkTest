// MIT License

// Copyright (c) 2022 Autodesk, Inc.

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#pragma once

#include "Roles/LiveLinkBasicRole.h"

#include "MayaLiveLinkAnimSequenceRole.generated.h"

/**
* Role associated for Animation timeline data.
*/
UCLASS(BlueprintType, meta = (DisplayName = "AnimSequence Role"))
class MAYALIVELINKINTERFACE_API UMayaLiveLinkAnimSequenceRole : public ULiveLinkBasicRole
{
	GENERATED_BODY()

public:
	//~ Begin ULiveLinkRole interface
	virtual UScriptStruct* GetStaticDataStruct() const override;
	virtual UScriptStruct* GetFrameDataStruct() const override;
	virtual UScriptStruct* GetBlueprintDataStruct() const override { return nullptr; }

	virtual bool InitializeBlueprintData(const FLiveLinkSubjectFrameData& InSourceData,
										 FLiveLinkBlueprintDataStruct& OutBlueprintData) const override
	{ return false; }

	virtual FText GetDisplayName() const override;
	virtual bool IsStaticDataValid(const FLiveLinkStaticDataStruct& InStaticData,
								   bool& bOutShouldLogWarning) const override;
	virtual bool IsFrameDataValid(const FLiveLinkStaticDataStruct& InStaticData,
								  const FLiveLinkFrameDataStruct& InFrameData,
								  bool& bOutShouldLogWarning) const override;
	//~ End ULiveLinkRole interface
};
