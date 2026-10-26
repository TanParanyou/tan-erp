//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'create_opportunity_request.g.dart';

/// CreateOpportunityRequest
///
/// Properties:
/// * [customerId] 
/// * [primarySiteId] 
/// * [title] 
/// * [scopeSummary] 
/// * [workTypes] 
/// * [sourceCode] 
/// * [expectedBudget] 
/// * [currencyCode] 
/// * [targetDecisionDate] 
/// * [nextActionAtUtc] 
/// * [nextActionNote] 
@BuiltValue()
abstract class CreateOpportunityRequest implements Built<CreateOpportunityRequest, CreateOpportunityRequestBuilder> {
  @BuiltValueField(wireName: r'customerId')
  String? get customerId;

  @BuiltValueField(wireName: r'primarySiteId')
  String? get primarySiteId;

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

  CreateOpportunityRequest._();

  factory CreateOpportunityRequest([void updates(CreateOpportunityRequestBuilder b)]) = _$CreateOpportunityRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CreateOpportunityRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CreateOpportunityRequest> get serializer => _$CreateOpportunityRequestSerializer();
}

class _$CreateOpportunityRequestSerializer implements PrimitiveSerializer<CreateOpportunityRequest> {
  @override
  final Iterable<Type> types = const [CreateOpportunityRequest, _$CreateOpportunityRequest];

  @override
  final String wireName = r'CreateOpportunityRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CreateOpportunityRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.customerId != null) {
      yield r'customerId';
      yield serializers.serialize(
        object.customerId,
        specifiedType: const FullType(String),
      );
    }
    if (object.primarySiteId != null) {
      yield r'primarySiteId';
      yield serializers.serialize(
        object.primarySiteId,
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
  }

  @override
  Object serialize(
    Serializers serializers,
    CreateOpportunityRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CreateOpportunityRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'customerId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.customerId = valueDes;
          break;
        case r'primarySiteId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.primarySiteId = valueDes;
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
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CreateOpportunityRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CreateOpportunityRequestBuilder();
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


