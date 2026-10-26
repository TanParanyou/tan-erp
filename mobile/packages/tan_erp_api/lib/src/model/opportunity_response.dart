//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/customer_summary_response.dart';
import 'package:tan_erp_api/src/model/owner_summary_response.dart';
import 'package:tan_erp_api/src/model/site_summary_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/branch_summary_response.dart';
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'opportunity_response.g.dart';

/// OpportunityResponse
///
/// Properties:
/// * [id] 
/// * [code] 
/// * [title] 
/// * [scopeSummary] 
/// * [workTypes] 
/// * [sourceCode] 
/// * [expectedBudget] 
/// * [currencyCode] 
/// * [targetDecisionDate] 
/// * [nextActionAtUtc] 
/// * [nextActionNote] 
/// * [stage] 
/// * [rowVersion] 
/// * [createdAtUtc] 
/// * [customer] 
/// * [primarySite] 
/// * [branch] 
/// * [owner] 
@BuiltValue()
abstract class OpportunityResponse implements Built<OpportunityResponse, OpportunityResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'title')
  String? get title;

  @BuiltValueField(wireName: r'scopeSummary')
  String? get scopeSummary;

  @BuiltValueField(wireName: r'workTypes')
  BuiltList<String>? get workTypes;

  @BuiltValueField(wireName: r'sourceCode')
  String? get sourceCode;

  @BuiltValueField(wireName: r'expectedBudget')
  double? get expectedBudget;

  @BuiltValueField(wireName: r'currencyCode')
  String? get currencyCode;

  @BuiltValueField(wireName: r'targetDecisionDate')
  Date? get targetDecisionDate;

  @BuiltValueField(wireName: r'nextActionAtUtc')
  DateTime? get nextActionAtUtc;

  @BuiltValueField(wireName: r'nextActionNote')
  String? get nextActionNote;

  @BuiltValueField(wireName: r'stage')
  String? get stage;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  @BuiltValueField(wireName: r'customer')
  CustomerSummaryResponse? get customer;

  @BuiltValueField(wireName: r'primarySite')
  SiteSummaryResponse? get primarySite;

  @BuiltValueField(wireName: r'branch')
  BranchSummaryResponse? get branch;

  @BuiltValueField(wireName: r'owner')
  OwnerSummaryResponse? get owner;

  OpportunityResponse._();

  factory OpportunityResponse([void updates(OpportunityResponseBuilder b)]) = _$OpportunityResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(OpportunityResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<OpportunityResponse> get serializer => _$OpportunityResponseSerializer();
}

class _$OpportunityResponseSerializer implements PrimitiveSerializer<OpportunityResponse> {
  @override
  final Iterable<Type> types = const [OpportunityResponse, _$OpportunityResponse];

  @override
  final String wireName = r'OpportunityResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    OpportunityResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.code != null) {
      yield r'code';
      yield serializers.serialize(
        object.code,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.title != null) {
      yield r'title';
      yield serializers.serialize(
        object.title,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.scopeSummary != null) {
      yield r'scopeSummary';
      yield serializers.serialize(
        object.scopeSummary,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.workTypes != null) {
      yield r'workTypes';
      yield serializers.serialize(
        object.workTypes,
        specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
      );
    }
    if (object.sourceCode != null) {
      yield r'sourceCode';
      yield serializers.serialize(
        object.sourceCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.expectedBudget != null) {
      yield r'expectedBudget';
      yield serializers.serialize(
        object.expectedBudget,
        specifiedType: const FullType.nullable(double),
      );
    }
    if (object.currencyCode != null) {
      yield r'currencyCode';
      yield serializers.serialize(
        object.currencyCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.targetDecisionDate != null) {
      yield r'targetDecisionDate';
      yield serializers.serialize(
        object.targetDecisionDate,
        specifiedType: const FullType.nullable(Date),
      );
    }
    if (object.nextActionAtUtc != null) {
      yield r'nextActionAtUtc';
      yield serializers.serialize(
        object.nextActionAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.nextActionNote != null) {
      yield r'nextActionNote';
      yield serializers.serialize(
        object.nextActionNote,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.stage != null) {
      yield r'stage';
      yield serializers.serialize(
        object.stage,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.rowVersion != null) {
      yield r'rowVersion';
      yield serializers.serialize(
        object.rowVersion,
        specifiedType: const FullType(String),
      );
    }
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.customer != null) {
      yield r'customer';
      yield serializers.serialize(
        object.customer,
        specifiedType: const FullType(CustomerSummaryResponse),
      );
    }
    if (object.primarySite != null) {
      yield r'primarySite';
      yield serializers.serialize(
        object.primarySite,
        specifiedType: const FullType(SiteSummaryResponse),
      );
    }
    if (object.branch != null) {
      yield r'branch';
      yield serializers.serialize(
        object.branch,
        specifiedType: const FullType(BranchSummaryResponse),
      );
    }
    if (object.owner != null) {
      yield r'owner';
      yield serializers.serialize(
        object.owner,
        specifiedType: const FullType(OwnerSummaryResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    OpportunityResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required OpportunityResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.code = valueDes;
          break;
        case r'title':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.title = valueDes;
          break;
        case r'scopeSummary':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.scopeSummary = valueDes;
          break;
        case r'workTypes':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
          ) as BuiltList<String>?;
          if (valueDes == null) continue;
          result.workTypes.replace(valueDes);
          break;
        case r'sourceCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.sourceCode = valueDes;
          break;
        case r'expectedBudget':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.expectedBudget = valueDes;
          break;
        case r'currencyCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.currencyCode = valueDes;
          break;
        case r'targetDecisionDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.targetDecisionDate = valueDes;
          break;
        case r'nextActionAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.nextActionAtUtc = valueDes;
          break;
        case r'nextActionNote':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.nextActionNote = valueDes;
          break;
        case r'stage':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.stage = valueDes;
          break;
        case r'rowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.rowVersion = valueDes;
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        case r'customer':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(CustomerSummaryResponse),
          ) as CustomerSummaryResponse?;
          if (valueDes == null) continue;
          result.customer.replace(valueDes);
          break;
        case r'primarySite':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(SiteSummaryResponse),
          ) as SiteSummaryResponse?;
          if (valueDes == null) continue;
          result.primarySite.replace(valueDes);
          break;
        case r'branch':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BranchSummaryResponse),
          ) as BranchSummaryResponse?;
          if (valueDes == null) continue;
          result.branch.replace(valueDes);
          break;
        case r'owner':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(OwnerSummaryResponse),
          ) as OwnerSummaryResponse?;
          if (valueDes == null) continue;
          result.owner.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  OpportunityResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = OpportunityResponseBuilder();
    final serializedList = (serialized as Iterable<Object?>).toList();
    final unhandled = <Object?>[];
    _deserializeProperties(
      serializers,
      serialized,
      specifiedType: specifiedType,
      serializedList: serializedList,
      unhandled: unhandled,
      result: result,
    );
    return result.build();
  }
}


